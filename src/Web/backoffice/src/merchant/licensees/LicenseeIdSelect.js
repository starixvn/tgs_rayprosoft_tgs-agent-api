import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useLicenseeList } from "./client"
import { useTrans } from "../../i18n"

export const LicenseeIdSelect = ({
    resellerId = "",
    placeholderDisabled = false,
    ...props
}) => {
    let { data, isLoading } = useLicenseeList({ showAll: true, resellerId })

    let placeholder = useLicenseePlaceholder()
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
        data?.items.map((licensee) => ({
            label: licensee.code,
            value: licensee.id
        })) ?? []
    return <Select options={[placeholderOption, ...options]} {...props} />
}

const useLicenseePlaceholder = () =>
    useTrans({
        zh: "选择公司",
        en: "Select Licensee"
    })

export default LicenseeIdSelect
