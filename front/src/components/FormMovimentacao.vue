<script setup>
    import { onMounted, ref } from 'vue';
    import { useEstoqueStore } from '@/stores/estoque';
    import { useProdutoStore } from '@/stores/produto';
    import { useArmazemStore } from '@/stores/armazem';
    import { useMensagemStore } from '@/stores/mensagem';
    import { storeToRefs } from 'pinia';
    import { obrigatorio, maiorQueZero } from '@/utils/formulario';

    const storeEstoque = useEstoqueStore();
    const storeProduto = useProdutoStore();
    const storeArmazem = useArmazemStore();
    const mensagemStore = useMensagemStore();
    const { produtos } = storeToRefs(storeProduto);
    const { armazens } = storeToRefs(storeArmazem);

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref({
        produtoId: null,
        armazemId: null,
        tipo: 'ENTRADA',
        quantidade: 1
    });

    // Carrega as opções dos campos de seleção
    onMounted(async () => {
        try {
            await Promise.all([storeProduto.listarProdutos(), storeArmazem.listarArmazens()]);
        } catch (error) {
            mensagemStore.mostrarErro(error);
        }
    });

    const limpar = () => {
        formulario.value.reset();
        item.value = { produtoId: null, armazemId: null, tipo: 'ENTRADA', quantidade: 1 };
    }

    const registrar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = await storeEstoque.registrarMovimentacao({
                ...item.value,
                quantidade: Number(item.value.quantidade)
            });
            mensagemStore.mostrarSucesso(`${resposta.mensagem} Novo saldo: ${resposta.dados.quantidade} un.`);
            limpar();
        } catch (error) {
            mensagemStore.mostrarErro(error);
        } finally {
            salvando.value = false;
        }
    }
</script>

<template>
    <v-card class="mb-6" elevation="2">
        <v-card-title class="text-h6">
            <v-icon icon="mdi-swap-vertical" class="mr-2"></v-icon>
            Registrar movimentação
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="registrar">
                <v-row dense>
                    <v-col cols="12" md="4">
                        <v-autocomplete v-model="item.produtoId" :items="produtos" item-title="descricao"
                            item-value="id" label="Produto *" :rules="[obrigatorio]" density="compact"></v-autocomplete>
                    </v-col>
                    <v-col cols="12" md="3">
                        <v-select v-model="item.armazemId" :items="armazens" item-title="nome"
                            item-value="id" label="Armazém *" :rules="[obrigatorio]" density="compact"></v-select>
                    </v-col>
                    <v-col cols="12" sm="6" md="3">
                        <v-radio-group v-model="item.tipo" inline density="compact">
                            <v-radio label="Entrada" value="ENTRADA" color="success"></v-radio>
                            <v-radio label="Saída" value="SAIDA" color="error"></v-radio>
                        </v-radio-group>
                    </v-col>
                    <v-col cols="12" sm="6" md="2">
                        <v-text-field v-model="item.quantidade" type="number" min="1" label="Quantidade *"
                            :rules="[maiorQueZero]" density="compact"></v-text-field>
                    </v-col>
                </v-row>
                <v-btn type="submit" class="mr-2" color="success" prepend-icon="mdi-content-save" :loading="salvando">Registrar</v-btn>
                <v-btn type="button" @click="limpar">Limpar</v-btn>
            </v-form>
        </v-card-text>
    </v-card>
</template>
