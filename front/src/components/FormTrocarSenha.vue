<script setup>
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';
    import { useAuthStore } from '@/stores/auth';
    import { useMensagemStore } from '@/stores/mensagem';
    import { senhaValida } from '@/utils/formulario';

    const authStore = useAuthStore();
    const mensagemStore = useMensagemStore();
    const router = useRouter();

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref({
        novaSenha: '',
        confirmacaoSenha: ''
    });

    const senhasIguais = (valor) => valor === item.value.novaSenha || 'As senhas não conferem';

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = await authStore.trocarSenha(item.value.novaSenha, item.value.confirmacaoSenha);
            mensagemStore.mostrarSucesso(resposta.mensagem);
            router.push(authStore.paginaInicial);
        } catch (error) {
            mensagemStore.mostrarErro(error);
        } finally {
            salvando.value = false;
        }
    }

    const sair = () => {
        authStore.sair();
        mensagemStore.mostrarSucesso('Você saiu do sistema.');
        router.push('/');
    }
</script>

<template>
    <v-row class="mt-8">
        <v-col cols="12" lg="4" md="6" offset-lg="4" offset-md="3">
            <v-card elevation="6">
                <v-card-title class="text-h6 pt-4">
                    <v-icon icon="mdi-lock-reset" class="mr-2"></v-icon>
                    Criar nova senha
                </v-card-title>
                <v-card-text>
                    <v-alert type="info" density="compact" class="mb-4">
                        Este é o seu primeiro acesso. Crie uma nova senha para continuar.
                    </v-alert>
                    <v-form ref="formulario" @submit.prevent="salvar">
                        <v-text-field type="password" v-model="item.novaSenha" label="Nova senha *"
                            :rules="[senhaValida]" variant="outlined" density="compact"></v-text-field>
                        <v-text-field type="password" v-model="item.confirmacaoSenha" label="Confirme a senha *"
                            :rules="[senhasIguais]" variant="outlined" density="compact"></v-text-field>
                        <v-btn type="submit" color="primary" block :loading="salvando">Salvar</v-btn>
                        <v-btn type="button" variant="text" block class="mt-2" @click="sair">Sair</v-btn>
                    </v-form>
                </v-card-text>
            </v-card>
        </v-col>
    </v-row>
</template>
