import { defineStore } from "pinia"
import { ref, computed } from "vue"
import api from "@/plugin/axios"

export const useAuthStore = defineStore("auth", () => {
    // Recupera o login salvo para não perder a sessão ao recarregar a página
    const token = ref(localStorage.getItem("token"));
    const usuario = ref(JSON.parse(localStorage.getItem("usuario") ?? "null"));

    const estaLogado = computed(() => !!token.value);
    const ehAdministrador = computed(() => usuario.value?.perfil === "ADMIN");
    const ehOperador = computed(() => usuario.value?.perfil === "OPERADOR");

    // Primeiro acesso: ainda está com a senha temporária
    const precisaTrocarSenha = computed(() => !!usuario.value?.trocarSenha);

    // O operador não acessa os cadastros, então começa pelo estoque
    const paginaInicial = computed(() => {
        if (precisaTrocarSenha.value) return "/trocar-senha";
        return ehOperador.value ? "/estoque" : "/transportadoras";
    });

    const entrar = async (email, senha) => {
        const response = await api.post("/autenticacao/login", { email, senha });
        guardarLogin(response.data.dados);
        return response.data;
    }

    const trocarSenha = async (novaSenha, confirmacaoSenha) => {
        const response = await api.put("/autenticacao/senha", { novaSenha, confirmacaoSenha });
        guardarLogin(response.data.dados);
        return response.data;
    }

    const guardarLogin = (dados) => {
        token.value = dados.token;
        usuario.value = dados.usuario;
        localStorage.setItem("token", token.value);
        localStorage.setItem("usuario", JSON.stringify(usuario.value));
    }

    const sair = () => {
        token.value = null;
        usuario.value = null;
        localStorage.removeItem("token");
        localStorage.removeItem("usuario");
    }

    return { token, usuario, estaLogado, ehAdministrador, ehOperador, precisaTrocarSenha, paginaInicial,
        entrar, trocarSenha, sair }
});
