import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePaymentMethodList } from "./client"

export const PaymentMethodSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "支付方式",
        en: "Payment Method"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data, error } = usePaymentMethodList()

    let options = data?.items?.map(({ trans, code }) => ({
        label: trans?.[locale] ?? code,
        value: code
    }))

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}

export const PaymentMethodLabel = ({ value }) => {
    let locale = useLocale()
    let { isLoading, data, error } = usePaymentMethodList()
    if (isLoading) {
        return ""
    }

    let method = data?.items?.find((item) => {
        return item.id === value
    })

    return method?.trans?.[locale] ?? method?.code ?? ""
}
