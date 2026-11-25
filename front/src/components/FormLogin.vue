<script setup>
    import { ref } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import { useAuthStore } from '@/stores/auth';
    import { useMensagemStore } from '@/stores/mensagem';

    const authStore = useAuthStore();
    const mensagemStore = useMensagemStore();
    const route = useRoute();
    const router = useRouter();

    const item = ref({
        email: "",
        senha: ""
    });
    const carregando = ref(false);

    // Aviso quando o token expirou e o usuário foi mandado de volta para o login
    const sessaoExpirada = route.query.sessaoExpirada === '1';

    async function entrar() {
        carregando.value = true;
        try {
            const resposta = await authStore.entrar(item.value.email, item.value.senha);
            mensagemStore.mostrarSucesso(resposta.mensagem);
            router.push(authStore.paginaInicial);
        } catch (error) {
            mensagemStore.mostrarErro(error);
        } finally {
            carregando.value = false;
        }
    }
</script>

<template>
    <v-row class="mt-8">
        <v-col cols="12" lg="4" md="6" offset-lg="4" offset-md="3">
            <v-card elevation="6" class="mx-auto">
                <v-card-title class="text-h6 pt-4">
                    <v-icon icon="mdi-truck-fast" class="mr-2"></v-icon>
                    Acesso ao sistema
                </v-card-title>
                <v-card-text>
                    <v-alert v-if="sessaoExpirada" type="warning" density="compact" class="mb-4">
                        Sua sessão expirou. Faça login novamente.
                    </v-alert>
                    <v-form @submit.prevent="entrar">
                        <v-text-field type="email" v-model="item.email" label="E-mail"
                            prepend-inner-icon="mdi-email" variant="outlined" density="compact"></v-text-field>
                        <v-text-field type="password" v-model="item.senha" label="Senha"
                            prepend-inner-icon="mdi-lock" variant="outlined" density="compact"></v-text-field>
                        <v-btn type="submit" color="primary" block :loading="carregando">
                            Entrar</v-btn>
                    </v-form>
                </v-card-text>
            </v-card>
        </v-col>
    </v-row>
</template>
