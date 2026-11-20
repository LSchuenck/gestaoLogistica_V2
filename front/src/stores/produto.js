import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useProdutoStore = defineStore("produto", () => {
    const produtos = ref([]);

    const listarProdutos = async () => {
        const response = await api.get('/produtos');
        produtos.value = response.data.dados;
        return response.data;
    }

    const cadastrarProduto = async (produto) => {
        const response = await api.post('/produtos', produto);
        produtos.value.push(response.data.dados);
        return response.data;
    }

    const alterarProduto = async (produto) => {
        const response = await api.put(`/produtos/${produto.id}`, produto);
        const indice = produtos.value.findIndex(p => p.id === produto.id);
        if (indice !== -1) {
            produtos.value[indice] = response.data.dados;
        }
        return response.data;
    }

    const excluirProduto = async (id) => {
        const response = await api.delete(`/produtos/${id}`);
        produtos.value = produtos.value.filter(p => p.id !== id);
        return response.data;
    }

    return { produtos, listarProdutos, cadastrarProduto, alterarProduto, excluirProduto }
});
