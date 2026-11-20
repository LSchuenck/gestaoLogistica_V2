<script setup>
import FormCliente from './FormCliente.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useClienteStore } from '@/stores/cliente';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';

const storeCliente = useClienteStore();
const mensagemStore = useMensagemStore();
const { clientes } = storeToRefs(storeCliente);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const clienteSelecionado = ref(null);
const dialogoExcluir = ref(false);
const clienteParaExcluir = ref(null);

const cabecalhos = [
    { title: 'Nome', key: 'nome' },
    { title: 'CPF/CNPJ', key: 'cpfCnpj' },
    { title: 'Telefone', key: 'telefone' },
    { title: 'Cidade/UF', key: 'cidade', value: c => `${c.endereco.cidade ?? ''} / ${c.endereco.estado ?? ''}` },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeCliente.listarClientes();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    clienteSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (cliente) => {
    clienteSelecionado.value = cliente;
    dialogoFormulario.value = true;
}

const confirmarExclusao = (cliente) => {
    clienteParaExcluir.value = cliente;
    dialogoExcluir.value = true;
}

const excluirCliente = async () => {
    try {
        const resposta = await storeCliente.excluirCliente(clienteParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-account-group" class="mr-2"></v-icon>
            Clientes
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="clientes" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.acoes="{ item }">
                    <v-btn @click="abrirEdicao(item)" icon="mdi-pencil" color="success"
                        density="comfortable" variant="text" title="Editar"></v-btn>
                    <v-btn @click="confirmarExclusao(item)" icon="mdi-delete" color="error"
                        density="comfortable" variant="text" title="Excluir"></v-btn>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>

    <v-dialog v-model="dialogoFormulario" max-width="700" persistent>
        <FormCliente v-if="dialogoFormulario" :cliente="clienteSelecionado"
            @fechar="dialogoFormulario = false"></FormCliente>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir cliente"
        :texto="`Deseja excluir o cliente ${clienteParaExcluir?.nome}?`"
        @confirmar="excluirCliente"></DialogoConfirmacao>
</template>
