<script setup>
import FormProduto from './FormProduto.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { computed, onMounted, ref } from 'vue';
import { useProdutoStore } from '@/stores/produto';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarData, formatarNumero } from '@/utils/formatacao';

const storeProduto = useProdutoStore();
const mensagemStore = useMensagemStore();
const { produtos } = storeToRefs(storeProduto);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const produtoSelecionado = ref(null);
const dialogoExcluir = ref(false);
const produtoParaExcluir = ref(null);

// Indicadores do topo (mesma ideia do sistema original)
const totalCriticos = computed(() => produtos.value.filter(p => p.estoqueCritico).length);
const totalVencidos = computed(() => produtos.value.filter(p => p.vencido).length);

const cabecalhos = [
    { title: 'Descrição', key: 'descricao' },
    { title: 'Peso (kg)', key: 'peso' },
    { title: 'Volume (m³)', key: 'volume' },
    { title: 'Validade', key: 'validade' },
    { title: 'Qtd. em estoque', key: 'quantidadeTotal' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeProduto.listarProdutos();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    produtoSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (produto) => {
    produtoSelecionado.value = produto;
    dialogoFormulario.value = true;
}

const confirmarExclusao = (produto) => {
    produtoParaExcluir.value = produto;
    dialogoExcluir.value = true;
}

const excluirProduto = async () => {
    try {
        const resposta = await storeProduto.excluirProduto(produtoParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-row class="mb-2">
        <v-col cols="12" sm="4">
            <v-card color="primary" variant="tonal">
                <v-card-text>Produtos cadastrados<div class="text-h5">{{ produtos.length }}</div></v-card-text>
            </v-card>
        </v-col>
        <v-col cols="12" sm="4">
            <v-card color="warning" variant="tonal">
                <v-card-text>Estoque crítico (menos de 10 un.)<div class="text-h5">{{ totalCriticos }}</div></v-card-text>
            </v-card>
        </v-col>
        <v-col cols="12" sm="4">
            <v-card color="error" variant="tonal">
                <v-card-text>Vencidos<div class="text-h5">{{ totalVencidos }}</div></v-card-text>
            </v-card>
        </v-col>
    </v-row>

    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-package-variant-closed" class="mr-2"></v-icon>
            Produtos
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="produtos" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.peso="{ item }">{{ formatarNumero(item.peso) }}</template>
                <template v-slot:item.volume="{ item }">{{ formatarNumero(item.volume) }}</template>
                <template v-slot:item.validade="{ item }">
                    {{ formatarData(item.validade) }}
                    <v-chip v-if="item.vencido" color="error" size="small" class="ml-1">Vencido</v-chip>
                </template>
                <template v-slot:item.quantidadeTotal="{ item }">
                    <v-chip :color="item.estoqueCritico ? 'warning' : 'success'" size="small">
                        {{ formatarNumero(item.quantidadeTotal) }}
                    </v-chip>
                </template>
                <template v-slot:item.acoes="{ item }">
                    <v-btn @click="abrirEdicao(item)" icon="mdi-pencil" color="success"
                        density="comfortable" variant="text" title="Editar"></v-btn>
                    <v-btn @click="confirmarExclusao(item)" icon="mdi-delete" color="error"
                        density="comfortable" variant="text" title="Excluir"></v-btn>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>

    <v-dialog v-model="dialogoFormulario" max-width="650" persistent>
        <FormProduto v-if="dialogoFormulario" :produto="produtoSelecionado"
            @fechar="dialogoFormulario = false"></FormProduto>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir produto"
        :texto="`Deseja excluir o produto ${produtoParaExcluir?.descricao}?`"
        @confirmar="excluirProduto"></DialogoConfirmacao>
</template>
