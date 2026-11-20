import { defineStore } from "pinia"
import { ref, computed } from "vue"
import api from "@/plugin/axios"

export const useAuthStore = defineStore("auth", () => {
    // Recupera o login salvo para não perder a sessão ao recarregar a página
    const token = ref(localStorage.getItem("token"));
    const usuario = ref(JSON.parse(localStorage.getItem("usuario") ?? "null"));

    const estaLogado = computed(() => !!token.value);
    const ehAdministrador = computed(() => usuario.value?.perfil === "ADMIN");

    const entrar = async (email, senha) => {
        const response = await api.post("/autenticacao/login", { email, senha });
        token.value = response.data.dados.token;
        usuario.value = response.data.dados.usuario;
        localStorage.setItem("token", token.value);
        localStorage.setItem("usuario", JSON.stringify(usuario.value));
        return response.data;
    }

    const sair = () => {
        token.value = null;
        usuario.value = null;
        localStorage.removeItem("token");
        localStorage.removeItem("usuario");
    }

    return { token, usuario, estaLogado, ehAdministrador, entrar, sair }
});
