import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useTransportadoraStore = defineStore("transportadora", () => {
    const transportadoras = ref([]);

    // Exibir todas (esta consulta usa cache na API)
    const listarTransportadoras = async () => {
        const response = await api.get('/transportadoras');
        transportadoras.value = response.data.dados;
        return response.data;
    }

    // Inserir
    const cadastrarTransportadora = async (transportadora) => {
        const response = await api.post('/transportadoras', transportadora);
        transportadoras.value.push(response.data.dados);
        return response.data;
    }

    // Alterar
    const alterarTransportadora = async (transportadora) => {
        const response = await api.put(`/transportadoras/${transportadora.id}`, transportadora);
        substituirNaLista(response.data.dados);
        return response.data;
    }

    // Ativar / desativar
    const alternarStatusTransportadora = async (id) => {
        const response = await api.patch(`/transportadoras/${id}/status`);
        substituirNaLista(response.data.dados);
        return response.data;
    }

    // Deletar
    const excluirTransportadora = async (id) => {
        const response = await api.delete(`/transportadoras/${id}`);
        transportadoras.value = transportadoras.value.filter(t => t.id !== id);
        return response.data;
    }

    const substituirNaLista = (transportadora) => {
        const indice = transportadoras.value.findIndex(t => t.id === transportadora.id);
        if (indice !== -1) {
            transportadoras.value[indice] = transportadora;
        }
    }

    return { transportadoras, listarTransportadoras, cadastrarTransportadora, alterarTransportadora,
        alternarStatusTransportadora, excluirTransportadora }
});
