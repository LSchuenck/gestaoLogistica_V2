<script setup>
import FormTransportadora from './FormTransportadora.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useTransportadoraStore } from '@/stores/transportadora';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';

const storeTransportadora = useTransportadoraStore();
const mensagemStore = useMensagemStore();
const { transportadoras } = storeToRefs(storeTransportadora);

const busca = ref('');
const carregando = ref(false);
const origemDados = ref(''); // mostra se a lista veio do cache ou do banco

const dialogoFormulario = ref(false);
const transportadoraSelecionada = ref(null);
const dialogoExcluir = ref(false);
const transportadoraParaExcluir = ref(null);

const cabecalhos = [
    { title: 'Nome fantasia', key: 'nomeFantasia' },
    { title: 'CNPJ', key: 'cnpj' },
    { title: 'Cidade/UF', key: 'cidade', value: t => `${t.endereco.cidade ?? ''} / ${t.endereco.estado ?? ''}` },
    { title: 'Telefone', key: 'telefone' },
    { title: 'Status', key: 'status' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(() => {
    carregarTransportadoras();
});

const carregarTransportadoras = async () => {
    carregando.value = true;
    try {
        const resposta = await storeTransportadora.listarTransportadoras();
        origemDados.value = resposta.mensagem;
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
}

const abrirCadastro = () => {
    transportadoraSelecionada.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (transportadora) => {
    transportadoraSelecionada.value = transportadora;
    dialogoFormulario.value = true;
}

const alternarStatus = async (transportadora) => {
    try {
        const resposta = await storeTransportadora.alternarStatusTransportadora(transportadora.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}

const confirmarExclusao = (transportadora) => {
    transportadoraParaExcluir.value = transportadora;
    dialogoExcluir.value = true;
}

const excluirTransportadora = async () => {
    try {
        const resposta = await storeTransportadora.excluirTransportadora(transportadoraParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-domain" class="mr-2"></v-icon>
            Transportadoras
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Nova</v-btn>
        </v-card-title>
        <v-card-subtitle>
            <v-icon icon="mdi-database-clock" size="small"></v-icon> {{ origemDados }}
            <v-btn variant="text" size="small" icon="mdi-refresh" title="Recarregar" @click="carregarTransportadoras"></v-btn>
        </v-card-subtitle>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="transportadoras" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.status="{ item }">
                    <v-chip :color="item.status === 'ATIVA' ? 'success' : 'grey'" size="small">{{ item.status }}</v-chip>
                </template>
                <template v-slot:item.acoes="{ item }">
                    <v-btn @click="abrirEdicao(item)" icon="mdi-pencil" color="success"
                        density="comfortable" variant="text" title="Editar"></v-btn>
                    <v-btn @click="alternarStatus(item)" :icon="item.status === 'ATIVA' ? 'mdi-toggle-switch' : 'mdi-toggle-switch-off'"
                        color="primary" density="comfortable" variant="text" title="Ativar/Desativar"></v-btn>
                    <v-btn @click="confirmarExclusao(item)" icon="mdi-delete" color="error"
                        density="comfortable" variant="text" title="Excluir"></v-btn>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>

    <v-dialog v-model="dialogoFormulario" max-width="700" persistent>
        <FormTransportadora v-if="dialogoFormulario" :transportadora="transportadoraSelecionada"
            @fechar="dialogoFormulario = false"></FormTransportadora>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir transportadora"
        :texto="`Deseja excluir a transportadora ${transportadoraParaExcluir?.nomeFantasia}?`"
        @confirmar="excluirTransportadora"></DialogoConfirmacao>
</template>
