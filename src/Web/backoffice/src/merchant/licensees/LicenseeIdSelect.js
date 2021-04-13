import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useLicenseeList } from "./client"
import { useTrans } from "../../i18n"

export const LicenseeIdSelect = ({ placeholder, resellerId, ...props }) => {
    let defaultPlaceholder = useLicenseePlaceholder()
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useLicenseeList({
        showAll: true,
        resellerId
    })
    let options = data?.items.map((licensee) => ({
        label: licensee.code,
        value: licensee.id
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

const useLicenseePlaceholder = () =>
    useTrans({
        zh: "选择公司",
        en: "Select Licensee"
    })

export default LicenseeIdSelect
