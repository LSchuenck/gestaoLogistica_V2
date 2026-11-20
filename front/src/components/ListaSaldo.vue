<script setup>
import { onMounted, ref } from 'vue';
import { useEstoqueStore } from '@/stores/estoque';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';

const storeEstoque = useEstoqueStore();
const mensagemStore = useMensagemStore();
const { saldos } = storeToRefs(storeEstoque);

const busca = ref('');
const carregando = ref(false);

const cabecalhos = [
    { title: 'Produto', key: 'produto' },
    { title: 'Armazém', key: 'armazem' },
    { title: 'Quantidade', key: 'quantidade', align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeEstoque.listarSaldos();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="text-h6">Saldo por armazém</v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar produto ou armazém" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>
            <v-data-table :headers="cabecalhos" :items="saldos" :search="busca" :loading="carregando"
                density="compact" hover>
                <template v-slot:item.quantidade="{ item }">
                    <v-chip :color="item.quantidade < 10 ? 'warning' : 'success'" size="small">{{ item.quantidade }}</v-chip>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>
</template>
