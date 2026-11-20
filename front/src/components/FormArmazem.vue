<script setup>
    import { ref } from 'vue';
    import { useArmazemStore } from '@/stores/armazem';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, copiarObjeto } from '@/utils/formulario';
    import FormEndereco from './FormEndereco.vue';

    const props = defineProps(['armazem']);
    const emit = defineEmits(['fechar']);

    const storeArmazem = useArmazemStore();
    const mensagemStore = useMensagemStore();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.armazem ? copiarObjeto(props.armazem) : {
        id: 0,
        nome: '',
        endereco: {}
    });

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = item.value.id
                ? await storeArmazem.alterarArmazem(item.value)
                : await storeArmazem.cadastrarArmazem(item.value);
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
            {{ item.id ? 'Editar armazém' : 'Novo armazém' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-text-field v-model="item.nome" label="Nome do armazém / CD *" :rules="[obrigatorio]" density="compact"></v-text-field>
                <FormEndereco v-model="item.endereco"></FormEndereco>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
