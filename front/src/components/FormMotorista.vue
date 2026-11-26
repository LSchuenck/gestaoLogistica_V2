<script setup>
    import { computed, onMounted, ref } from 'vue';
    import { useMotoristaStore } from '@/stores/motorista';
    import { useTransportadoraStore } from '@/stores/transportadora';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, copiarObjeto, aplicarMascara } from '@/utils/formulario';
    import { mascaraCpf, mascaraTelefone } from '@/utils/formatacao';

    const props = defineProps(['motorista']);
    const emit = defineEmits(['fechar']);

    const storeMotorista = useMotoristaStore();
    const storeTransportadora = useTransportadoraStore();
    const mensagemStore = useMensagemStore();

    const categoriasCnh = ['A', 'B', 'C', 'D', 'E', 'AB', 'AC', 'AD', 'AE'];

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.motorista ? copiarObjeto(props.motorista) : {
        id: 0,
        transportadoraId: null,
        nome: '',
        cpf: '',
        cnh: '',
        categoriaCnh: null,
        validadeCnh: '',
        telefone: ''
    });

    // Só transportadoras ativas podem receber motoristas
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

        // Campo de data vazio precisa ir como null para a API
        const dados = { ...item.value, validadeCnh: item.value.validadeCnh || null };

        salvando.value = true;
        try {
            const resposta = dados.id
                ? await storeMotorista.alterarMotorista(dados)
                : await storeMotorista.cadastrarMotorista(dados);
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
            {{ item.id ? 'Editar motorista' : 'Novo motorista' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-row dense>
                    <v-col cols="12">
                        <v-select v-model="item.transportadoraId" :items="transportadorasAtivas"
                            item-title="nomeFantasia" item-value="id" label="Transportadora *"
                            :rules="[obrigatorio]" density="compact"></v-select>
                    </v-col>
                    <v-col cols="12">
                        <v-text-field v-model="item.nome" label="Nome *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.cpf" @update:model-value="aplicarMascara(item, 'cpf', mascaraCpf)" maxlength="14" label="CPF" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.telefone" @update:model-value="aplicarMascara(item, 'telefone', mascaraTelefone)" maxlength="15" label="Telefone" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="5">
                        <v-text-field v-model="item.cnh" label="CNH *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="3">
                        <v-select v-model="item.categoriaCnh" :items="categoriasCnh" label="Categoria" density="compact"></v-select>
                    </v-col>
                    <v-col cols="12" sm="4">
                        <v-text-field v-model="item.validadeCnh" type="date" label="Validade da CNH" density="compact"></v-text-field>
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
