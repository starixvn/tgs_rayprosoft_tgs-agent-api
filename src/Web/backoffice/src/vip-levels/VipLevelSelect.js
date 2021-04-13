import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"
import Select from "../select/Select"
import { useTrans } from "../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const VipLevelSelect = ({
    placeholder,
    placeholderDisabled,
    brandId,
    optionValue,
    ...props
}) => {
    let formMethods = useFormContext()
    let defaultPlaceholder = useTrans({
        zh: "选择 VIP 等级",
        en: "Select VIP Level"
    })

    let { isLoading, data, error, mutate, deleteCache } = useVipLevelList({
        brandId,
        showAll: true
    })

    if (props.name && formMethods && isLoading) {
        return (
            <Controller
                name={props.name}
                control={formMethods.control}
                defaultValue={props.defaultValue}
                render={({ field, fieldState, formState }) => {
                    return <Select isLoading={true} options={[]} />
                }}
            />
        )
    }

    let vipLevelOptions =
        data?.items?.map((item) => {
            let value = optionValue ? optionValue(item) : item?.id
            return {
                label: item.name,
                value
            }
        }) ?? []

    placeholder = placeholder ?? defaultPlaceholder
    let options =
        typeof placeholder === "string"
            ? [
                  {
                      label: placeholder,
                      value: "",
                      disabled: placeholderDisabled
                  },
                  ...vipLevelOptions
              ]
            : vipLevelOptions

    return (
        <>
            <Select isLoading={isLoading} options={options} {...props} />
        </>
    )
}

export default VipLevelSelect
