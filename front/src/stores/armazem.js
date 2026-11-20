import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useArmazemStore = defineStore("armazem", () => {
    const armazens = ref([]);

    const listarArmazens = async () => {
        const response = await api.get('/armazens');
        armazens.value = response.data.dados;
        return response.data;
    }

    const cadastrarArmazem = async (armazem) => {
        const response = await api.post('/armazens', armazem);
        armazens.value.push(response.data.dados);
        return response.data;
    }

    const alterarArmazem = async (armazem) => {
        const response = await api.put(`/armazens/${armazem.id}`, armazem);
        const indice = armazens.value.findIndex(a => a.id === armazem.id);
        if (indice !== -1) {
            armazens.value[indice] = response.data.dados;
        }
        return response.data;
    }

    const excluirArmazem = async (id) => {
        const response = await api.delete(`/armazens/${id}`);
        armazens.value = armazens.value.filter(a => a.id !== id);
        return response.data;
    }

    return { armazens, listarArmazens, cadastrarArmazem, alterarArmazem, excluirArmazem }
});
