import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Select as _Select } from "bumbag"
import AlertDialog from "../dialog/Alert"
import { useVipLevelList } from "./client"
import Select from "../select/Select"
import { useTrans } from "../i18n"

export const VipLevelSelect = ({
    placeholder,
    placeholderDisabled,
    brandId,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "选择 VIP 等级",
        en: "Select VIP Level"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useVipLevelList({ brandId, showAll: true })

    let vipLevelOptions =
        data?.items?.map(({ id, ...item }) => ({
            label: item.name,
            value: id
        })) ?? []

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
            {error && <AlertDialog {...error} />}
        </>
    )
}

export default VipLevelSelect
