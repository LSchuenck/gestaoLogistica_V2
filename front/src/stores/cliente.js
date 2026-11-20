import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useClienteStore = defineStore("cliente", () => {
    const clientes = ref([]);

    const listarClientes = async () => {
        const response = await api.get('/clientes');
        clientes.value = response.data.dados;
        return response.data;
    }

    const cadastrarCliente = async (cliente) => {
        const response = await api.post('/clientes', cliente);
        clientes.value.push(response.data.dados);
        return response.data;
    }

    const alterarCliente = async (cliente) => {
        const response = await api.put(`/clientes/${cliente.id}`, cliente);
        const indice = clientes.value.findIndex(c => c.id === cliente.id);
        if (indice !== -1) {
            clientes.value[indice] = response.data.dados;
        }
        return response.data;
    }

    const excluirCliente = async (id) => {
        const response = await api.delete(`/clientes/${id}`);
        clientes.value = clientes.value.filter(c => c.id !== id);
        return response.data;
    }

    return { clientes, listarClientes, cadastrarCliente, alterarCliente, excluirCliente }
});
