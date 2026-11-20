<script setup>
import FormMotorista from './FormMotorista.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useMotoristaStore } from '@/stores/motorista';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarData } from '@/utils/formatacao';

const storeMotorista = useMotoristaStore();
const mensagemStore = useMensagemStore();
const { motoristas } = storeToRefs(storeMotorista);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const motoristaSelecionado = ref(null);
const dialogoExcluir = ref(false);
const motoristaParaExcluir = ref(null);

const cabecalhos = [
    { title: 'Nome', key: 'nome' },
    { title: 'Transportadora', key: 'nomeTransportadora' },
    { title: 'CNH', key: 'cnh' },
    { title: 'Categoria', key: 'categoriaCnh' },
    { title: 'Validade CNH', key: 'validadeCnh' },
    { title: 'Telefone', key: 'telefone' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeMotorista.listarMotoristas();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    motoristaSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (motorista) => {
    motoristaSelecionado.value = motorista;
    dialogoFormulario.value = true;
}

const confirmarExclusao = (motorista) => {
    motoristaParaExcluir.value = motorista;
    dialogoExcluir.value = true;
}

const excluirMotorista = async () => {
    try {
        const resposta = await storeMotorista.excluirMotorista(motoristaParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-card-account-details" class="mr-2"></v-icon>
            Motoristas
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="motoristas" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.validadeCnh="{ item }">
                    {{ formatarData(item.validadeCnh) }}
                    <v-chip v-if="item.cnhVencida" color="error" size="small" class="ml-1">Vencida</v-chip>
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
        <FormMotorista v-if="dialogoFormulario" :motorista="motoristaSelecionado"
            @fechar="dialogoFormulario = false"></FormMotorista>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir motorista"
        :texto="`Deseja excluir o motorista ${motoristaParaExcluir?.nome}?`"
        @confirmar="excluirMotorista"></DialogoConfirmacao>
</template>
