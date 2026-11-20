<script setup>
import FormArmazem from './FormArmazem.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useArmazemStore } from '@/stores/armazem';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';

const storeArmazem = useArmazemStore();
const mensagemStore = useMensagemStore();
const { armazens } = storeToRefs(storeArmazem);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const armazemSelecionado = ref(null);
const dialogoExcluir = ref(false);
const armazemParaExcluir = ref(null);

const cabecalhos = [
    { title: 'Nome', key: 'nome' },
    { title: 'Logradouro', key: 'logradouro', value: a => `${a.endereco.logradouro ?? ''}, ${a.endereco.numero ?? ''}` },
    { title: 'Cidade/UF', key: 'cidade', value: a => `${a.endereco.cidade ?? ''} / ${a.endereco.estado ?? ''}` },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeArmazem.listarArmazens();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    armazemSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (armazem) => {
    armazemSelecionado.value = armazem;
    dialogoFormulario.value = true;
}

const confirmarExclusao = (armazem) => {
    armazemParaExcluir.value = armazem;
    dialogoExcluir.value = true;
}

const excluirArmazem = async () => {
    try {
        const resposta = await storeArmazem.excluirArmazem(armazemParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-warehouse" class="mr-2"></v-icon>
            Armazéns
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="armazens" :search="busca"
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
        <FormArmazem v-if="dialogoFormulario" :armazem="armazemSelecionado"
            @fechar="dialogoFormulario = false"></FormArmazem>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir armazém"
        :texto="`Deseja excluir o armazém ${armazemParaExcluir?.nome}?`"
        @confirmar="excluirArmazem"></DialogoConfirmacao>
</template>
