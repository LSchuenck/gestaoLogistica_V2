<script setup>
    import { computed, onMounted, ref } from 'vue';
    import { useVeiculoStore } from '@/stores/veiculo';
    import { useTransportadoraStore } from '@/stores/transportadora';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, copiarObjeto } from '@/utils/formulario';

    const props = defineProps(['veiculo']);
    const emit = defineEmits(['fechar']);

    const storeVeiculo = useVeiculoStore();
    const storeTransportadora = useTransportadoraStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.veiculo ? copiarObjeto(props.veiculo) : {
        id: 0,
        transportadoraId: null,
        placa: '',
        modelo: '',
        tipoVeiculo: '',
        capacidadeCarga: 0
    });

    // Só transportadoras ativas podem receber veículos
    const transportadorasAtivas = computed(() =>
        storeTransportadora.transportadoras.filter(t => t.status === 'ATIVA'));

    onMounted(async () => {
        try {
            await storeTransportadora.listarTransportadoras();
        } catch (error) {
            mensagemStore.mostrarErro(error);
        }
    });

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        const dados = { ...item.value, capacidadeCarga: Number(item.value.capacidadeCarga) || 0 };

        salvando.value = true;
        try {
            const resposta = dados.id
                ? await storeVeiculo.alterarVeiculo(dados)
                : await storeVeiculo.cadastrarVeiculo(dados);
            mensagemStore.mostrarSucesso(resposta.mensagem);
            emit('fechar');
        } catch (error) {
            mensagemStore.mostrarErro(error);
        } finally {
            salvando.value = false;
        }
    }
</script>

<template>
    <v-card>
        <v-card-title class="text-h6">
            {{ item.id ? 'Editar veículo' : 'Novo veículo' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-row dense>
                    <v-col cols="12">
                        <v-select v-model="item.transportadoraId" :items="transportadorasAtivas"
                            item-title="nomeFantasia" item-value="id" label="Transportadora *"
                            :rules="[obrigatorio]" density="compact"></v-select>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.placa" label="Placa *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="8">
                        <v-text-field v-model="item.modelo" label="Modelo" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.tipoVeiculo" label="Tipo (ex.: Carreta Baú)" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.capacidadeCarga" type="number" min="0" label="Capacidade de carga (kg)" density="compact"></v-text-field>
                    </v-col>
                </v-row>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
