import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { Button } from "bumbag"
import { useForm, useFormContext } from "react-hook-form"
import { useTrans } from "../i18n"
import { Input, InputField } from "./Input"

//Note: ((10^6)-1)/(10^18) -> 1,000,000,000,000,000,000?

export const NumberInput = ({
    type = "number",
    max = "10000000000",
    ...props
}) => <Input type={type} max={max} {...props} />

export const NumberInputField = ({
    type = "number",
    max = "10000000000",
    ...props
}) => <InputField type={type} max={max} {...props} />

export const PercentageNumberInput = () => {}

const DefaultPercentageAddon = () => (
    <Button isStatic tabIndex="-1">
        %
    </Button>
)

export const PercentageNumberInputField = ({
    addonAfter,
    min = "0",
    max = "100",
    step = "0.01",
    ...props
}) => {
    addonAfter = addonAfter ?? <DefaultPercentageAddon />
    return (
        <NumberInputField
            step={step}
            min={min}
            max={max}
            addonAfter={addonAfter}
            {...props}
        />
    )
}
