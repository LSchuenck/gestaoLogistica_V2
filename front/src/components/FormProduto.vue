<script setup>
    import { ref } from 'vue';
    import { useProdutoStore } from '@/stores/produto';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, copiarObjeto } from '@/utils/formulario';

    const props = defineProps(['produto']);
    const emit = defineEmits(['fechar']);

    const storeProduto = useProdutoStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.produto ? copiarObjeto(props.produto) : {
        id: 0,
        descricao: '',
        peso: null,
        volume: null,
        validade: ''
    });

    // Converte campo vazio em null (a API espera número ou nada)
    const numeroOuNulo = (valor) => (valor === '' || valor === null ? null : Number(valor));

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        const dados = {
            ...item.value,
            peso: numeroOuNulo(item.value.peso),
            volume: numeroOuNulo(item.value.volume),
            validade: item.value.validade || null
        };

        salvando.value = true;
        try {
            const resposta = dados.id
                ? await storeProduto.alterarProduto(dados)
                : await storeProduto.cadastrarProduto(dados);
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
            {{ item.id ? 'Editar produto' : 'Novo produto' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-row dense>
                    <v-col cols="12">
                        <v-text-field v-model="item.descricao" label="Descrição *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.peso" type="number" min="0" step="0.01" label="Peso (kg)" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.volume" type="number" min="0" step="0.01" label="Volume (m³)" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.validade" type="date" label="Validade" density="compact"></v-text-field>
                    </v-col>
                </v-row>
                <div class="text-caption mb-2">O saldo do produto é controlado na tela de Estoque.</div>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
