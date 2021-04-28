import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePaymentChannelList } from "./client"

//TODO: filter by payment id and payment method id
export const PaymentChannelSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "支付通道",
        en: "Payment Channel"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data, error } = usePaymentChannelList()

    let options = data?.map?.(({ trans, code, id }) => ({
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

export const PaymentChannelLabel = ({ value }) => {
    let locale = useLocale()
    let { isLoading, data, error } = usePaymentChannelList()
    if (isLoading) {
        return ""
    }

    let method = data?.find((item) => {
        return item.id === value
    })

    return method?.trans?.[locale] ?? method?.code ?? ""
}
