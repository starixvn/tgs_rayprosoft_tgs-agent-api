import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePaymentMethodList } from "./client"

export const PaymentMethodSelect = ({ range, placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "支付方式",
        en: "Payment Method"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data, error } = usePaymentMethodList()

    let options = data
        ?.filter((item) => {
            if (range) {
                return range.includes(item.id)
            }
            return true
        })
        .map(({ trans, code, id }) => ({
            label: trans?.[locale] ?? code,
            value: id
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

    let method = data?.find((item) => {
        return item.id === value
    })

    return method?.trans?.[locale] ?? method?.code ?? ""
}
