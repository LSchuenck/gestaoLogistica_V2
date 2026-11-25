<script setup>
import FormViagem from './FormViagem.vue';
import DialogoConfirmacao from './DialogoConfirmacao.vue';
import { onMounted, ref } from 'vue';
import { useViagemStore } from '@/stores/viagem';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';
import { formatarDataHora, formatarMoeda, formatarNumero } from '@/utils/formatacao';

const storeViagem = useViagemStore();
const mensagemStore = useMensagemStore();
const { viagens } = storeToRefs(storeViagem);

const busca = ref('');
const carregando = ref(false);

const dialogoFormulario = ref(false);
const dialogoCancelar = ref(false);
const viagemParaCancelar = ref(null);

// Texto e cor de cada status
const statusViagem = {
    INICIADA: { texto: 'Iniciada', cor: 'info' },
    EM_TRANSITO: { texto: 'Em trânsito', cor: 'warning' },
    CONCLUIDA: { texto: 'Concluída', cor: 'success' },
    CANCELADA: { texto: 'Cancelada', cor: 'grey' },
};

const cabecalhos = [
    { title: 'Nº', key: 'id' },
    { title: 'Origem / Destino / Carga', key: 'origem', value: v => `${v.origem} → ${v.destino} ${v.produto}` },
    { title: 'Veículo / Motorista', key: 'veiculo', value: v => `${v.veiculo} - ${v.motorista}` },
    { title: 'Saída / Chegada', key: 'dataSaida' },
    { title: 'Faturamento', key: 'faturamento' },
    { title: 'Status', key: 'status' },
    { title: 'Ações', key: 'acoes', sortable: false, align: 'end' },
];

onMounted(async () => {
    carregando.value = true;
    try {
        await storeViagem.listarViagens();
    } catch (error) {
        mensagemStore.mostrarErro(error);
    } finally {
        carregando.value = false;
    }
});

const emAndamento = (viagem) => viagem.status === 'INICIADA' || viagem.status === 'EM_TRANSITO';

const avancarViagem = (viagem) => executar(() => storeViagem.avancarViagem(viagem.id));
const concluirViagem = (viagem) => executar(() => storeViagem.concluirViagem(viagem.id));
const cancelarViagem = () => executar(() => storeViagem.cancelarViagem(viagemParaCancelar.value.id));

// Executa a ação e mostra a mensagem da API
const executar = async (acao) => {
    try {
        const resposta = await acao();
        mensagemStore.mostrarSucesso(resposta.mensagem);
    } catch (error) {
        mensagemStore.mostrarErro(error);
    }
}

const confirmarCancelamento = (viagem) => {
    viagemParaCancelar.value = viagem;
    dialogoCancelar.value = true;
}
</script>

<template>
    <v-card elevation="2">
        <v-card-title class="d-flex align-center">
            <v-icon icon="mdi-truck-fast" class="mr-2"></v-icon>
            Viagens
            <v-spacer></v-spacer>
            <v-btn color="primary" prepend-icon="mdi-plus" @click="dialogoFormulario = true">Nova</v-btn>
        </v-card-title>
        <v-card-text>
            <v-text-field v-model="busca" label="Pesquisar" prepend-inner-icon="mdi-magnify"
                variant="outlined" density="compact" hide-details class="mb-4"></v-text-field>

            <v-data-table :headers="cabecalhos" :items="viagens" :search="busca" :loading="carregando"
                :sort-by="[{ key: 'id', order: 'desc' }]" item-value="id" hover>
                <template v-slot:item.origem="{ item }">
                    <div>{{ item.origem }} → {{ item.destino }}</div>
                    <div v-if="item.produto" class="text-caption">{{ item.quantidade }} un. de {{ item.produto }}</div>
                </template>
                <template v-slot:item.dataSaida="{ item }">
                    <div class="text-no-wrap">{{ formatarDataHora(item.dataSaida) }}</div>
                    <div class="text-no-wrap">{{ formatarDataHora(item.dataChegada) }}</div>
                </template>
                <template v-slot:item.faturamento="{ item }">
                    <div class="text-no-wrap">{{ formatarMoeda(item.faturamento) }}</div>
                    <div class="text-caption">{{ formatarNumero(item.distanciaKm) }} km x {{ formatarMoeda(item.valorPorKm) }}</div>
                </template>
                <template v-slot:item.status="{ item }">
                    <v-chip :color="statusViagem[item.status].cor" size="small">{{ statusViagem[item.status].texto }}</v-chip>
                </template>
                <template v-slot:item.acoes="{ item }">
                    <div v-if="emAndamento(item)" class="d-flex justify-end align-center ga-1">
                        <v-btn size="x-small" color="primary" :disabled="item.status !== 'INICIADA'"
                            @click="avancarViagem(item)">Avançar</v-btn>
                        <v-btn size="x-small" color="success" :disabled="item.status !== 'EM_TRANSITO'"
                            @click="concluirViagem(item)">Concluir</v-btn>
                        <v-btn size="x-small" color="error" variant="text" icon="mdi-close" title="Cancelar viagem"
                            @click="confirmarCancelamento(item)"></v-btn>
                    </div>
                </template>
            </v-data-table>
        </v-card-text>
    </v-card>

    <v-dialog v-model="dialogoFormulario" max-width="550" persistent>
        <FormViagem v-if="dialogoFormulario" @fechar="dialogoFormulario = false"></FormViagem>
    </v-dialog>

    <DialogoConfirmacao v-model="dialogoCancelar" titulo="Cancelar viagem"
        :texto="`Deseja cancelar a viagem nº ${viagemParaCancelar?.id}?`"
        @confirmar="cancelarViagem"></DialogoConfirmacao>
</template>
