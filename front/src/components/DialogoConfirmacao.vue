<script setup>
    // Janela de confirmação reaproveitada em todas as exclusões
    const visivel = defineModel({ type: Boolean });
    defineProps(['titulo', 'texto']);
    const emit = defineEmits(['confirmar']);

    const confirmar = () => {
        visivel.value = false;
        emit('confirmar');
    }
</script>

<template>
    <v-dialog v-model="visivel" max-width="450">
        <v-card>
            <v-card-title class="text-h6">{{ titulo }}</v-card-title>
            <v-card-text>{{ texto }}</v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn @click="visivel = false">Cancelar</v-btn>
                <v-btn color="error" variant="flat" @click="confirmar">Confirmar</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
