<script setup>
import FormMovimentacao from '@/components/FormMovimentacao.vue';
import ListaSaldo from '@/components/ListaSaldo.vue';
import ListaMovimentacao from '@/components/ListaMovimentacao.vue';
</script>

<template>
    <FormMovimentacao></FormMovimentacao>
    <v-row>
        <v-col cols="12" lg="5">
            <ListaSaldo></ListaSaldo>
        </v-col>
        <v-col cols="12" lg="7">
            <ListaMovimentacao></ListaMovimentacao>
        </v-col>
    </v-row>
</template>
