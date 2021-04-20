import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { CheckboxGroup as _CheckboxGroup } from "bumbag"

export const CheckboxGroup = ({ name, defaultValue = [], ...props }) => {
    let { control } = useFormContext()

    return (
        <Controller
            control={control}
            name={name}
            defaultValue={defaultValue}
            render={({
                field: { onChange, onBlur, value, name, ref },
                fieldState: { invalid, isTouched, isDirty }
            }) => (
                <_CheckboxGroup
                    onBlur={onBlur}
                    onChange={(value) => {
                        onChange(value)
                    }}
                    value={value}
                    {...props}
                />
            )}
        />
    )
}

export default CheckboxGroup
