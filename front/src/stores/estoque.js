import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useEstoqueStore = defineStore("estoque", () => {
    const saldos = ref([]);
    const movimentacoes = ref([]);

    // Saldo de cada produto em cada armazém
    const listarSaldos = async () => {
        const response = await api.get('/estoque');
        saldos.value = response.data.dados;
        return response.data;
    }

    // Últimas movimentações (histórico)
    const listarMovimentacoes = async () => {
        const response = await api.get('/estoque/movimentacoes');
        movimentacoes.value = response.data.dados;
        return response.data;
    }

    // Registra ENTRADA ou SAIDA e recarrega as duas listas
    const registrarMovimentacao = async (movimentacao) => {
        const response = await api.post('/estoque/movimentacoes', movimentacao);
        await Promise.all([listarSaldos(), listarMovimentacoes()]);
        return response.data;
    }

    return { saldos, movimentacoes, listarSaldos, listarMovimentacoes, registrarMovimentacao }
});
