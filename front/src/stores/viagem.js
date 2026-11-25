import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useViagemStore = defineStore("viagem", () => {
    const viagens = ref([]);

    const listarViagens = async () => {
        const response = await api.get('/viagens');
        viagens.value = response.data.dados;
        return response.data;
    }

    const iniciarViagem = async (viagem) => {
        const response = await api.post('/viagens', viagem);
        viagens.value.push(response.data.dados);
        return response.data;
    }

    const avancarViagem = (id) => mudarStatus(id, 'avancar');
    const concluirViagem = (id) => mudarStatus(id, 'concluir');
    const cancelarViagem = (id) => mudarStatus(id, 'cancelar');

    // Os três passos são iguais: chamam a API e atualizam a linha da tabela
    const mudarStatus = async (id, acao) => {
        const response = await api.patch(`/viagens/${id}/${acao}`);
        const indice = viagens.value.findIndex(v => v.id === id);
        if (indice !== -1) {
            viagens.value[indice] = response.data.dados;
        }
        return response.data;
    }

    return { viagens, listarViagens, iniciarViagem, avancarViagem, concluirViagem, cancelarViagem }
});
