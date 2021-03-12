import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useResellerList } from "./client"
import { useTrans } from "../../i18n"

export const ResellerIdSelect = ({ placeholderDisabled = false, ...props }) => {
    let { data, isLoading } = useResellerList({ showAll: true })

    let placeholder = useResellerPlaceholder()
    let placeholderOption = {
        label: placeholder,
        value: "",
        disabled: placeholderDisabled
    }

    if (isLoading) {
        return (
            <Select
                isLoading={isLoading}
                options={[placeholderOption]}
                {...props}
            />
        )
    }

    let options =
        data?.items.map((reseller) => ({
            label: reseller.code,
            value: reseller.id
        })) ?? []

    return <Select options={[placeholderOption, ...options]} {...props} />
}

const useResellerPlaceholder = () =>
    useTrans({
        zh: "选择总公司",
        en: "Select Reseller"
    })

export default ResellerIdSelect
