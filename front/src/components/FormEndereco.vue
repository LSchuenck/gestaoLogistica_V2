<script setup>
    import { estados } from '@/utils/formatacao';

    // Campos de endereço usados nos formulários de transportadora, cliente e armazém
    const endereco = defineModel({ type: Object });
</script>

<template>
    <div class="text-subtitle-2 mb-2">Endereço</div>
    <v-row dense>
        <v-col cols="12" sm="4">
            <v-text-field v-model="endereco.cep" label="CEP" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="8">
            <v-text-field v-model="endereco.logradouro" label="Logradouro" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="4">
            <v-text-field v-model="endereco.numero" label="Número" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="8">
            <v-text-field v-model="endereco.complemento" label="Complemento" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="5">
            <v-text-field v-model="endereco.bairro" label="Bairro" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="5">
            <v-text-field v-model="endereco.cidade" label="Cidade" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="2">
            <v-select v-model="endereco.estado" :items="estados" label="UF" density="compact"></v-select>
        </v-col>
    </v-row>
</template>
