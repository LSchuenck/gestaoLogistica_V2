<script setup>
    import { computed, onMounted, ref, watch } from 'vue';
    import { useViagemStore } from '@/stores/viagem';
    import { useArmazemStore } from '@/stores/armazem';
    import { useClienteStore } from '@/stores/cliente';
    import { useVeiculoStore } from '@/stores/veiculo';
    import { useMotoristaStore } from '@/stores/motorista';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio } from '@/utils/formulario';

    const emit = defineEmits(['fechar']);

    const storeViagem = useViagemStore();
    const storeArmazem = useArmazemStore();
    const storeCliente = useClienteStore();
    const storeVeiculo = useVeiculoStore();
    const storeMotorista = useMotoristaStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref({
        armazemId: null,
        clienteId: null,
        veiculoId: null,
        motoristaId: null
    });

    const veiculosDisponiveis = computed(() =>
        storeVeiculo.veiculos.filter(v => v.status === 'DISPONIVEL'));

    // Motoristas da mesma transportadora do veículo, com a CNH em dia
    const motoristasDoVeiculo = computed(() => {
        const veiculo = storeVeiculo.veiculos.find(v => v.id === item.value.veiculoId);
        return storeMotorista.motoristas.filter(m => m.transportadoraId === veiculo?.transportadoraId && !m.cnhVencida);
    });

    // Ao trocar o veículo, o motorista precisa ser escolhido de novo
    watch(() => item.value.veiculoId, () => item.value.motoristaId = null);

    onMounted(async () => {
        try {
            await Promise.all([storeArmazem.listarArmazens(), storeCliente.listarClientes(),
                storeVeiculo.listarVeiculos(), storeMotorista.listarMotoristas()]);
        } catch (error) {
            mensagemStore.mostrarErro(error);
        }
    });

    const iniciar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = await storeViagem.iniciarViagem(item.value);
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
        <v-card-title class="text-h6">Nova viagem</v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="iniciar">
                <v-select v-model="item.armazemId" :items="storeArmazem.armazens" item-title="nome" item-value="id"
                    label="Origem (armazém) *" :rules="[obrigatorio]" density="compact"></v-select>
                <v-select v-model="item.clienteId" :items="storeCliente.clientes" item-title="nome" item-value="id"
                    label="Destino (cliente) *" :rules="[obrigatorio]" density="compact"></v-select>
                <v-select v-model="item.veiculoId" :items="veiculosDisponiveis" item-title="placa" item-value="id"
                    label="Veículo disponível *" :rules="[obrigatorio]" density="compact"></v-select>
                <v-select v-model="item.motoristaId" :items="motoristasDoVeiculo" item-title="nome" item-value="id"
                    label="Motorista *" :rules="[obrigatorio]" :disabled="!item.veiculoId" density="compact"
                    no-data-text="Nenhum motorista com CNH válida nesta transportadora"></v-select>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-truck-fast" :loading="salvando">Iniciar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
