import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import { Switch } from "../../switch/Switch"
import { Select } from "../../select/Select"
import { useChangePromotionStatus } from "./client"
import { Update } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import AlertDialog from "../../dialog/Alert"

const Enabled = "Enabled"
const EnabledTrans = {
    zh: "开启",
    en: "Enabled"
}
const Disabled = "Disabled"
const DisabledTrans = {
    zh: "关闭",
    en: "Disabled"
}

export const PromotionStatusSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "状态",
        en: "Status"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(EnabledTrans),
            value: Enabled
        },
        {
            label: useTrans(DisabledTrans),
            value: Disabled
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export const PromotionStatusSwitch = ({
    record,
    value,
    disabled,
    onSubmitted,
    ...props
}) => {
    let editable = useIsDisplayMenuByRoute([Update])
    if (!editable) {
        disabled = true
    }

    let [formData, setSubmit] = useState()
    let {
        isLoading,
        result,
        error,
        mutate,
        deleteCache
    } = useChangePromotionStatus(record?.id, formData)

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted?.()
        }
    }, [result, error])

    let checked = value === Enabled
    return (
        <>
            <Switch
                disabled={disabled}
                checked={checked}
                {...props}
                css={css`
                    position: relative;
                `}
                onChange={(event) => {
                    if (disabled) {
                        return
                    }

                    let checked = event.target.checked
                    if (checked) {
                        setSubmit(Enabled)
                    } else {
                        setSubmit(Disabled)
                    }
                }}
            />
            {error && <AlertDialog {...error} onClickClose={reset} />}
        </>
    )
}
