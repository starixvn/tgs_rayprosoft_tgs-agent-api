import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../i18n"
import Select from "../select/Select"
import { usePaymentList } from "./client"

export const PaymentProviderSelect = ({ placeholder, range, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "支付厂商",
        en: "Payment Provider"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data, error } = usePaymentList()

    let options = data
        ?.filter((item) => {
            if (range) {
                return range.includes(item.id)
            }
            return true
        })
        ?.map(({ trans, code, id }) => ({
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

export const PaymentProviderLabel = ({ value }) => {
    let locale = useLocale()
    let { isLoading, data, error } = usePaymentList()
    if (isLoading) {
        return ""
    }

    let method = data?.find((item) => {
        return item.id === value
    })

    return method?.trans?.[locale] ?? method?.code ?? ""
}
