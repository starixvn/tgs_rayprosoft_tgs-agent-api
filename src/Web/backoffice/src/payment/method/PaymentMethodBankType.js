import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePaymentMethodList } from "./client"

export const Bank = "Bank"
const BankTrans = {
    zh: "银行",
    en: "Bank"
}

export const Crypto = "Crypto"
const CryptoTrans = {
    zh: "数字货币",
    en: "Crypto"
}

export const isBank = (value) => value === Bank

export const isCrypto = (value) => value === Crypto

export const PaymentMethodBankTypeSelect = ({
    placeholder,
    brandId,
    range = [],
    isLoading,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "支付类型",
        en: "Payment Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let {
        isLoading: isLoadingPaymentMethodList,
        data,
        error
    } = usePaymentMethodList()

    let options = data
        ?.filter((item) => {
            if (range?.length > 0) {
                return range.includes(item.id)
            }
            return true
        })
        ?.map(({ trans, code, bankType }) => ({
            label: trans?.[locale] ?? code,
            value: bankType
        }))

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading || isLoadingPaymentMethodList}
            options={options}
            {...props}
        />
    )
}

/*
export const PaymentMethodBankTypeLabel = ({ value }) => {
    let { isLoading, data, error } = usePaymentMethodList()
    let locale = useLocale()
    let paymentMethod = data?.find?.((item) => item.id === value)

    //TODO: diff trans for bank type?
    return paymentMethod?.trans?.[locale] ?? paymentMethod?.bankType ?? ""
}
*/

export default {
    Bank,
    Crypto,
    isBank,
    isCrypto
}
