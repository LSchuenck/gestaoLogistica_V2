import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useVeiculoStore = defineStore("veiculo", () => {
    const veiculos = ref([]);

    const listarVeiculos = async () => {
        const response = await api.get('/veiculos');
        veiculos.value = response.data.dados;
        return response.data;
    }

    const cadastrarVeiculo = async (veiculo) => {
        const response = await api.post('/veiculos', veiculo);
        veiculos.value.push(response.data.dados);
        return response.data;
    }

    const alterarVeiculo = async (veiculo) => {
        const response = await api.put(`/veiculos/${veiculo.id}`, veiculo);
        substituirNaLista(response.data.dados);
        return response.data;
    }

    // Status: DISPONIVEL, EM_VIAGEM ou MANUTENCAO
    const alterarStatusVeiculo = async (id, status) => {
        const response = await api.patch(`/veiculos/${id}/status`, { status });
        substituirNaLista(response.data.dados);
        return response.data;
    }

    const excluirVeiculo = async (id) => {
        const response = await api.delete(`/veiculos/${id}`);
        veiculos.value = veiculos.value.filter(v => v.id !== id);
        return response.data;
    }

    const substituirNaLista = (veiculo) => {
        const indice = veiculos.value.findIndex(v => v.id === veiculo.id);
        if (indice !== -1) {
            veiculos.value[indice] = veiculo;
        }
    }

    return { veiculos, listarVeiculos, cadastrarVeiculo, alterarVeiculo, alterarStatusVeiculo, excluirVeiculo }
});
