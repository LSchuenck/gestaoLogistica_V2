import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useUsuarioStore = defineStore("usuario", () => {
    const usuarios = ref([]);

    const listarUsuarios = async () => {
        const response = await api.get('/usuarios');
        usuarios.value = response.data.dados;
        return response.data;
    }

    const cadastrarUsuario = async (usuario) => {
        const response = await api.post('/usuarios', usuario);
        usuarios.value.push(response.data.dados);
        return response.data;
    }

    const alterarUsuario = async (usuario) => {
        const response = await api.put(`/usuarios/${usuario.id}`, usuario);
        substituirNaLista(response.data.dados);
        return response.data;
    }

    // Ativar / desativar
    const alternarStatusUsuario = async (id) => {
        const response = await api.patch(`/usuarios/${id}/status`);
        substituirNaLista(response.data.dados);
        return response.data;
    }

    const excluirUsuario = async (id) => {
        const response = await api.delete(`/usuarios/${id}`);
        usuarios.value = usuarios.value.filter(u => u.id !== id);
        return response.data;
    }

    const substituirNaLista = (usuario) => {
        const indice = usuarios.value.findIndex(u => u.id === usuario.id);
        if (indice !== -1) {
            usuarios.value[indice] = usuario;
        }
    }

    return { usuarios, listarUsuarios, cadastrarUsuario, alterarUsuario, alternarStatusUsuario, excluirUsuario }
});
