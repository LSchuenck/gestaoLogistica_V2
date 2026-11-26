<script setup>
    import { ref } from 'vue';
    import { useTransportadoraStore } from '@/stores/transportadora';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, emailValido, copiarObjeto, aplicarMascara } from '@/utils/formulario';
    import { mascaraCnpj, mascaraTelefone } from '@/utils/formatacao';
    import FormEndereco from './FormEndereco.vue';

    // Se receber uma transportadora é edição; se vier vazio é cadastro
    const props = defineProps(['transportadora']);
    const emit = defineEmits(['fechar']);

    const storeTransportadora = useTransportadoraStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.transportadora ? copiarObjeto(props.transportadora) : {
        id: 0,
        cnpj: '',
        razaoSocial: '',
        nomeFantasia: '',
        telefone: '',
        email: '',
        endereco: {}
    });

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = item.value.id
                ? await storeTransportadora.alterarTransportadora(item.value)
                : await storeTransportadora.cadastrarTransportadora(item.value);
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
            {{ item.id ? 'Editar transportadora' : 'Nova transportadora' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-row dense>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.cnpj" @update:model-value="aplicarMascara(item, 'cnpj', mascaraCnpj)" maxlength="18" label="CNPJ *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.nomeFantasia" label="Nome fantasia *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12">
                        <v-text-field v-model="item.razaoSocial" label="Razão social *" :rules="[obrigatorio]" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.telefone" @update:model-value="aplicarMascara(item, 'telefone', mascaraTelefone)" maxlength="15" label="Telefone" density="compact"></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                        <v-text-field v-model="item.email" label="E-mail" :rules="[emailValido]" density="compact"></v-text-field>
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
