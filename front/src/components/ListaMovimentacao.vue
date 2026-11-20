<script setup>
import { onMounted } from 'vue';
import { useEstoqueStore } from '@/stores/estoque';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarDataHora } from '@/utils/formatacao';

const storeEstoque = useEstoqueStore();
const mensagemStore = useMensagemStore();
const { movimentacoes } = storeToRefs(storeEstoque);

onMounted(async () => {
    try {
        await storeEstoque.listarMovimentacoes();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
});
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="text-h6">Últimas movimentações</v-card-title>
        <v-card-text>
            <v-table density="compact" hover>
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Tipo</th>
                        <th>Produto</th>
                        <th>Armazém</th>
                        <th>Qtd.</th>
                        <th>Usuário</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="movimentacao in movimentacoes" :key="movimentacao.id">
                        <td>{{ formatarDataHora(movimentacao.dataMovimentacao) }}</td>
                        <td>
                            <v-chip :color="movimentacao.tipo === 'ENTRADA' ? 'success' : 'error'" size="small">
                                {{ movimentacao.tipo }}
                            </v-chip>
                        </td>
                        <td>{{ movimentacao.produto }}</td>
                        <td>{{ movimentacao.armazem }}</td>
                        <td>{{ movimentacao.quantidade }}</td>
                        <td>{{ movimentacao.usuario }}</td>
                    </tr>
                    <tr v-if="movimentacoes.length === 0">
                        <td colspan="6" class="text-center text-medium-emphasis">Nenhuma movimentação registrada.</td>
                    </tr>
                </tbody>
            </v-table>
        </v-card-text>
    </v-card>
</template>
