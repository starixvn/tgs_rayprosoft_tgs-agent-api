import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { useLicenseeList } from "./client"
import { useTrans } from "../../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const LicenseeIdSelect = ({
    resellerId = "",
    placeholderDisabled = false,
    ...props
}) => {
    let formMethods = useFormContext()
    let { data, isLoading } = useLicenseeList({ showAll: true, resellerId })

    let placeholder = useLicenseePlaceholder()
    let placeholderOption = {
        label: placeholder,
        value: "",
        disabled: placeholderDisabled
    }

    if (
        isLoading &&
        props?.name &&
        props?.defaultValue &&
        formMethods?.control
    ) {
        return (
            <Controller
                name={props?.name}
                control={formMethods?.control}
                render={() => <Select isLoading={isLoading} options={[]} />}
                defaultValue={props?.defaultValue}
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
