<script setup>
import FormVeiculo from './FormVeiculo.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useVeiculoStore } from '@/stores/veiculo';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarNumero } from '@/utils/formatacao';

const storeVeiculo = useVeiculoStore();
const mensagemStore = useMensagemStore();
const { veiculos } = storeToRefs(storeVeiculo);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const veiculoSelecionado = ref(null);
const dialogoExcluir = ref(false);
const veiculoParaExcluir = ref(null);

// Status possíveis com o texto e a cor exibidos na tela
const statusVeiculo = [
    { valor: 'DISPONIVEL', texto: 'Disponível', cor: 'success' },
    { valor: 'EM_VIAGEM', texto: 'Em viagem', cor: 'info' },
    { valor: 'MANUTENCAO', texto: 'Manutenção', cor: 'warning' },
];
const buscarStatus = (valor) => statusVeiculo.find(s => s.valor === valor) ?? statusVeiculo[0];

const cabecalhos = [
    { title: 'Placa', key: 'placa' },
    { title: 'Modelo', key: 'modelo' },
    { title: 'Tipo', key: 'tipoVeiculo' },
    { title: 'Transportadora', key: 'nomeTransportadora' },
    { title: 'Capacidade (kg)', key: 'capacidadeCarga' },
    { title: 'Status', key: 'status' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeVeiculo.listarVeiculos();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const abrirCadastro = () => {
    veiculoSelecionado.value = null;
    dialogoFormulario.value = true;
}

const abrirEdicao = (veiculo) => {
    veiculoSelecionado.value = veiculo;
    dialogoFormulario.value = true;
}

const alterarStatus = async (veiculo, novoStatus) => {
    try {
        const resposta = await storeVeiculo.alterarStatusVeiculo(veiculo.id, novoStatus);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}

const confirmarExclusao = (veiculo) => {
    veiculoParaExcluir.value = veiculo;
    dialogoExcluir.value = true;
}

const excluirVeiculo = async () => {
    try {
        const resposta = await storeVeiculo.excluirVeiculo(veiculoParaExcluir.value.id);
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-truck" class="mr-2"></v-icon>
            Veículos
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="abrirCadastro">Novo</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="veiculos" :search="busca"
                :loading="carregando" item-value="id" hover>
                <template v-slot:item.capacidadeCarga="{ item }">
                    {{ formatarNumero(item.capacidadeCarga) }}
                </template>
                <template v-slot:item.status="{ item }">
                    <!-- Clicar no status abre o menu para trocar -->
                    <v-menu>
                        <template v-slot:activator="{ props }">
                            <v-chip v-bind="props" :color="buscarStatus(item.status).cor" size="small"
                                append-icon="mdi-menu-down">{{ buscarStatus(item.status).texto }}</v-chip>
                        </template>
                        <v-list density="compact">
                            <v-list-item v-for="status in statusVeiculo" :key="status.valor"
                                :title="status.texto" @click="alterarStatus(item, status.valor)"></v-list-item>
                        </v-list>
                    </v-menu>
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
        <FormVeiculo v-if="dialogoFormulario" :veiculo="veiculoSelecionado"
            @fechar="dialogoFormulario = false"></FormVeiculo>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoExcluir" titulo="Excluir veículo"
        :texto="`Deseja excluir o veículo de placa ${veiculoParaExcluir?.placa}?`"
        @confirmar="excluirVeiculo"></DialogoConfirmacao>
</template>
