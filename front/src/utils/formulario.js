// Regras de validação usadas nos campos dos formulários (Vuetify "rules")

export const obrigatorio = (valor) => (valor !== null && valor !== undefined && valor !== '') || 'Campo obrigatório';

export const emailValido = (valor) => !valor || /.+@.+\..+/.test(valor) || 'E-mail inválido';

export const maiorQueZero = (valor) => Number(valor) > 0 || 'Informe um valor maior que zero';

// Cria uma cópia do registro para editar sem alterar a lista antes de salvar
export function copiarObjeto(objeto) {
    return JSON.parse(JSON.stringify(objeto));
}
