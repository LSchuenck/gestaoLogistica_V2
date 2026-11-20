import { defineStore } from "pinia";
import { ref } from "vue";
import api from "@/plugin/axios";

export const useMotoristaStore = defineStore("motorista", () => {
    const motoristas = ref([]);

    const listarMotoristas = async () => {
        const response = await api.get('/motoristas');
        motoristas.value = response.data.dados;
        return response.data;
    }

    const cadastrarMotorista = async (motorista) => {
        const response = await api.post('/motoristas', motorista);
        motoristas.value.push(response.data.dados);
        return response.data;
    }

    const alterarMotorista = async (motorista) => {
        const response = await api.put(`/motoristas/${motorista.id}`, motorista);
        const indice = motoristas.value.findIndex(m => m.id === motorista.id);
        if (indice !== -1) {
            motoristas.value[indice] = response.data.dados;
        }
        return response.data;
    }

    const excluirMotorista = async (id) => {
        const response = await api.delete(`/motoristas/${id}`);
        motoristas.value = motoristas.value.filter(m => m.id !== id);
        return response.data;
    }

    return { motoristas, listarMotoristas, cadastrarMotorista, alterarMotorista, excluirMotorista }
});
