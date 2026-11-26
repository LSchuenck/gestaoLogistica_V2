<script setup>
    import { ref } from 'vue';
    import { useClienteStore } from '@/stores/cliente';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, copiarObjeto, aplicarMascara } from '@/utils/formulario';
    import { mascaraCpfCnpj, mascaraTelefone } from '@/utils/formatacao';
    import FormEndereco from './FormEndereco.vue';

    const props = defineProps(['cliente']);
    const emit = defineEmits(['fechar']);

    const storeCliente = useClienteStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.cliente ? copiarObjeto(props.cliente) : {
        id: 0,
        nome: '',
        cpfCnpj: '',
        telefone: '',
        endereco: {}
    });

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = item.value.id
                ? await storeCliente.alterarCliente(item.value)
                : await storeCliente.cadastrarCliente(item.value);
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
            {{ item.id ? 'Editar cliente' : 'Novo cliente' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-row dense>
                    <v-col cols="12">
                        <v-text-field v-model="item.nome" label="Nome *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.cpfCnpj" @update:model-value="aplicarMascara(item, 'cpfCnpj', mascaraCpfCnpj)" maxlength="18" label="CPF/CNPJ" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.telefone" @update:model-value="aplicarMascara(item, 'telefone', mascaraTelefone)" maxlength="15" label="Telefone" density="compact"></v-text-field>
                    </v-col>
                </v-row>
                <FormEndereco v-model="item.endereco"></FormEndereco>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
