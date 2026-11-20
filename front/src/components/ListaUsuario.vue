<script setup>
import FormUsuario from './FormUsuario.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useUsuarioStore } from '@/stores/usuario';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarDataHora } from '@/utils/formatacao';

const storeUsuario = useUsuarioStore();
const mensagemStore = useMensagemStore();
const { usuarios } = storeToRefs(storeUsuario);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const usuarioSelecionado = ref(null);
const dialogoExcluir = ref(false);
const usuarioParaExcluir = ref(null);

const cabecalhos = [
    { title: 'Nome', key: 'nome' },
    { title: 'E-mail', key: 'email' },
    { title: 'Perfil', key: 'perfil' },
    { title: 'Status', key: 'status' },
    { title: 'Cadastro', key: 'dataCadastro' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeUsuario.listarUsuarios();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    usuarioSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (usuario) => {
    usuarioSelecionado.value = usuario;
    dialogoFormulario.value = true;
}

const alternarStatus = async (usuario) => {
    try {
        const resposta = await storeUsuario.alternarStatusUsuario(usuario.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}

const confirmarExclusao = (usuario) => {
    usuarioParaExcluir.value = usuario;
    dialogoExcluir.value = true;
}

const excluirUsuario = async () => {
    try {
        const resposta = await storeUsuario.excluirUsuario(usuarioParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-account-cog" class="mr-2"></v-icon>
            Usuários
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="usuarios" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.status="{ item }">
                    <v-chip :color="item.status === 'ATIVO' ? 'success' : 'grey'" size="small">{{ item.status }}</v-chip>
                </template>
                <template v-slot:item.dataCadastro="{ item }">{{ formatarDataHora(item.dataCadastro) }}</template>
                <template v-slot:item.acoes="{ item }">
                    <v-btn @click="abrirEdicao(item)" icon="mdi-pencil" color="success"
                        density="comfortable" variant="text" title="Editar"></v-btn>
                    <v-btn @click="alternarStatus(item)" :icon="item.status === 'ATIVO' ? 'mdi-toggle-switch' : 'mdi-toggle-switch-off'"
                        color="primary" density="comfortable" variant="text" title="Ativar/Desativar"></v-btn>
                    <v-btn @click="confirmarExclusao(item)" icon="mdi-delete" color="error"
                        density="comfortable" variant="text" title="Excluir"></v-btn>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>

    <v-dialog v-model="dialogoFormulario" max-width="500" persistent>
        <FormUsuario v-if="dialogoFormulario" :usuario="usuarioSelecionado"
            @fechar="dialogoFormulario = false"></FormUsuario>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir usuário"
        :texto="`Deseja excluir o usuário ${usuarioParaExcluir?.nome}?`"
        @confirmar="excluirUsuario"></DialogoConfirmacao>
</template>
