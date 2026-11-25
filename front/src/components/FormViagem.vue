<script setup>
    import { computed, onMounted, ref, watch } from 'vue';
    import { useViagemStore } from '@/stores/viagem';
    import { useArmazemStore } from '@/stores/armazem';
    import { useClienteStore } from '@/stores/cliente';
    import { useVeiculoStore } from '@/stores/veiculo';
    import { useMotoristaStore } from '@/stores/motorista';
    import { useEstoqueStore } from '@/stores/estoque';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, maiorQueZero } from '@/utils/formulario';
    import { formatarMoeda } from '@/utils/formatacao';

    const emit = defineEmits(['fechar']);

    const storeViagem = useViagemStore();
    const storeArmazem = useArmazemStore();
    const storeCliente = useClienteStore();
    const storeVeiculo = useVeiculoStore();
    const storeMotorista = useMotoristaStore();
    const storeEstoque = useEstoqueStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref({
        armazemId: null,
        clienteId: null,
        veiculoId: null,
        motoristaId: null,
        produtoId: null,
        quantidade: '',
        distanciaKm: '',
        valorPorKm: ''
    });

    // Mostra o faturamento enquanto o usuário digita
    const faturamento = computed(() => Number(item.value.distanciaKm) * Number(item.value.valorPorKm));

    const veiculosDisponiveis = computed(() =>
        storeVeiculo.veiculos.filter(v => v.status === 'DISPONIVEL'));

    // Motoristas da mesma transportadora do veículo, com a CNH em dia
    const motoristasDoVeiculo = computed(() => {
        const veiculo = storeVeiculo.veiculos.find(v => v.id === item.value.veiculoId);
        return storeMotorista.motoristas.filter(m => m.transportadoraId === veiculo?.transportadoraId && !m.cnhVencida);
    });

    // Produtos com saldo no armazém de origem, com a quantidade disponível no texto
    const produtosDoArmazem = computed(() =>
        storeEstoque.saldos
            .filter(s => s.armazemId === item.value.armazemId)
            .map(s => ({ id: s.produtoId, texto: `${s.produto} (${s.quantidade} un.)` })));

    // Ao trocar o veículo, o motorista precisa ser escolhido de novo
    watch(() => item.value.veiculoId, () => item.value.motoristaId = null);

    // Ao trocar o armazém, o produto precisa ser escolhido de novo
    watch(() => item.value.armazemId, () => item.value.produtoId = null);

    onMounted(async () => {
        try {
            await Promise.all([storeArmazem.listarArmazens(), storeCliente.listarClientes(),
                storeVeiculo.listarVeiculos(), storeMotorista.listarMotoristas(), storeEstoque.listarSaldos()]);
        } catch (error) {
            mensagemStore.mostrarErro(error);
        }
    });

    const iniciar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = await storeViagem.iniciarViagem({
                ...item.value,
                quantidade: Number(item.value.quantidade),
                distanciaKm: Number(item.value.distanciaKm),
                valorPorKm: Number(item.value.valorPorKm)
            });
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
                <v-row density="compact">
                    <v-col cols="12" sm="8">
                        <v-select v-model="item.produtoId" :items="produtosDoArmazem" item-title="texto" item-value="id"
                            label="Produto *" :rules="[obrigatorio]" :disabled="!item.armazemId" density="compact"
                            no-data-text="Nenhum produto com saldo neste armazém"></v-select>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.quantidade" type="number" min="1" step="1" label="Quantidade *"
                            :rules="[maiorQueZero]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.distanciaKm" type="number" min="0" step="0.01" label="Distância (km) *"
                            :rules="[maiorQueZero]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.valorPorKm" type="number" min="0" step="0.01" label="Valor do frete por km (R$) *"
                            :rules="[maiorQueZero]" density="compact"></v-text-field>
                    </v-col>
                </v-row>
                <div>Faturamento da viagem: <strong>{{ formatarMoeda(faturamento) }}</strong></div>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-truck-fast" :loading="salvando">Iniciar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
