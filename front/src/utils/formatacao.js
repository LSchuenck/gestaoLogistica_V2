// Funções simples para exibir dados no formato brasileiro

// "2027-08-15" -> "15/08/2027"
export function formatarData(data) {
    if (!data) return '-';
    const [ano, mes, dia] = data.substring(0, 10).split('-');
    return `${dia}/${mes}/${ano}`;
}

// "2026-11-20T16:25:51" -> "20/11/2026 16:25"
export function formatarDataHora(dataHora) {
    if (!dataHora) return '-';
    return new Date(dataHora).toLocaleString('pt-BR', { dateStyle: 'short', timeStyle: 'short' });
}

// 42000 -> "42.000"
export function formatarNumero(numero) {
    if (numero === null || numero === undefined) return '-';
    return Number(numero).toLocaleString('pt-BR');
}

// 1290 -> "R$ 1.290,00"
export function formatarMoeda(valor) {
    return Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

// Lista de UFs para os campos de endereço
export const estados = ['AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MT', 'MS', 'MG',
    'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN', 'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'];

// Máscaras: deixam só os números e devolvem no formato certo
export const apenasNumeros = (valor) => String(valor ?? '').replace(/\D/g, '');
export const mascaraCep = (valor) => apenasNumeros(valor).slice(0, 8).replace(/^(\d{5})(\d)/, '$1-$2');
export const mascaraCpf = (valor) => apenasNumeros(valor).slice(0, 11)
    .replace(/^(\d{3})(\d)/, '$1.$2').replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3').replace(/\.(\d{3})(\d{1,2})$/, '.$1-$2');
export const mascaraCnpj = (valor) => apenasNumeros(valor).slice(0, 14)
    .replace(/^(\d{2})(\d)/, '$1.$2').replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
    .replace(/^(\d{2})\.(\d{3})\.(\d{3})(\d)/, '$1.$2.$3/$4').replace(/\/(\d{4})(\d)/, '/$1-$2');
export const mascaraCpfCnpj = (valor) => (apenasNumeros(valor).length <= 11 ? mascaraCpf(valor) : mascaraCnpj(valor));
export const mascaraTelefone = (valor) => {
    const numeros = apenasNumeros(valor).slice(0, 11);
    if (numeros.length <= 2) return numeros ? "(" + numeros : "";
    const ddd = numeros.slice(0, 2);
    const resto = numeros.slice(2);
    const corte = numeros.length === 11 ? 5 : 4; // celular tem 9 dígitos depois do DDD
    const inicio = resto.slice(0, corte);
    const fim = resto.slice(corte);
    return "(" + ddd + ") " + inicio + (fim ? "-" + fim : "");
};
