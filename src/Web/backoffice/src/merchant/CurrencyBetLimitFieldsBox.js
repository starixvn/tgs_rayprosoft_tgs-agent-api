import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack, Table } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import Checkbox from "../checkbox"
import Input from "../input/Input"
import { isAllChecked, isAllPropChecked } from "../checkbox/isAllChecked"
import { Switch } from "../switch/Switch"
import get from "lodash-es/get"

export const CurrencyBetLimitFieldsBox = ({
    currencyFieldName = "currencies",
    betLimitFieldName = "betLimits",
    width = "100%",
    showBetLimitDefault = false,
    readonly,
    ...props
}) => {
    let { control, watch } = useFormContext()

    let currencies = watch(currencyFieldName)
    if (!currencies || currencies.length == 0) {
        return null
    }

    return (
        <Body
            currencyFieldName={currencyFieldName}
            betLimitFieldName={betLimitFieldName}
            disabled={readonly}
            showBetLimitDefault={showBetLimitDefault}
            width={width}
        />
    )
}

const Body = ({
    currencyFieldName,
    betLimitFieldName,
    showBetLimitDefault,
    disabled,
    width = "100%"
}) => {
    let {
        control,
        watch,
        formState: { errors }
    } = useFormContext()

    let currencyFieldArray = useFieldArray({
        control,
        name: currencyFieldName,
        keyName: "_id"
    })
    let betLimitFieldArray = useFieldArray({
        control,
        name: betLimitFieldName,
        keyName: "_id"
    })

    let currencies = watch(currencyFieldName)
    let isAllCurrencyChecked = isAllPropChecked(currencies)

    let currencyBetLimitsPairs = currencyFieldArray?.fields?.map(
        (currencyField) => {
            let betLimitFields = betLimitFieldArray?.fields?.filter(
                ({ currencyCode }) => currencyField.code === currencyCode
            )
            return [currencyField, betLimitFields]
        }
    )

    return useMemo(() => {
        return (
            <>
                <NormalLabel>
                    <Trans zh="币种限红" en="Currency Bet Limit" />
                </NormalLabel>
                <Set orientation="vertical" width={width}>
                    <Table width={width} hasDividers>
                        <Table.Head>
                            <Table.Row>
                                <Table.HeadCell>
                                    <Box></Box>
                                </Table.HeadCell>
                                <Table.HeadCell
                                    colSpan={showBetLimitDefault ? "5" : "4"}
                                >
                                    <Trans zh="币种" en="Currency" />
                                </Table.HeadCell>
                            </Table.Row>
                        </Table.Head>
                        <Table.Body>
                            {currencyBetLimitsPairs.map(
                                ([currencyField, betLimitFields], index) => {
                                    return (
                                        <CurrencyBetLimitsRow
                                            key={currencyField._id}
                                            currencyField={currencyField}
                                            currencyFieldIndex={index}
                                            currencyFieldName={
                                                currencyFieldName
                                            }
                                            betLimitFields={betLimitFields}
                                            showBetLimitDefault={
                                                showBetLimitDefault
                                            }
                                            disabled={disabled}
                                        />
                                    )
                                }
                            )}
                        </Table.Body>
                    </Table>
                </Set>
            </>
        )
    }, [isAllCurrencyChecked, JSON.stringify(currencyBetLimitsPairs), errors])
}

const toCurrencyCheckedFieldName = (name, index) => {
    return `${name}.${index}.checked`
}

const CurrencyBetLimitsRow = ({
    currencyField,
    currencyFieldIndex,
    currencyFieldName,
    betLimitFields = [],
    showBetLimitDefault = false,
    disabled
}) => {
    let { watch } = useFormContext()

    let currencyCheckedFieldName = toCurrencyCheckedFieldName(
        currencyFieldName,
        currencyFieldIndex
    )

    let isCurrencyChecked = watch(currencyCheckedFieldName)
    let betLimitHeaderVisibility = isCurrencyChecked ? "visible" : "hidden"

    let currencyFieldRow = useMemo(() => {
        return (
            <>
                <Table.Row key={currencyField._id}>
                    <Table.HeadCell>
                        <Box>
                            <Checkbox
                                name={currencyCheckedFieldName}
                                defaultChecked={currencyField.checked}
                                disabled={disabled}
                            />
                            <Input
                                type="hidden"
                                name={`${currencyFieldName}.${currencyFieldIndex}.code`}
                                value={currencyField.code}
                            />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell>{currencyField.code}</Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="最低投注" en="Min Bet" />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="最高投注" en="Max Bet" />
                        </Box>
                    </Table.HeadCell>
                    <Table.HeadCell textAlign="center">
                        <Box visibility={betLimitHeaderVisibility}>
                            <Trans zh="单场最高投注" en="Max Bet Per Match" />
                        </Box>
                    </Table.HeadCell>
                    {showBetLimitDefault && (
                        <Table.HeadCell textAlign="center">
                            <Box visibility={betLimitHeaderVisibility}>
                                <Trans zh="预设" en="Default" />
                            </Box>
                        </Table.HeadCell>
                    )}
                </Table.Row>
            </>
        )
    }, [isCurrencyChecked])

    return (
        <>
            {currencyFieldRow}
            {betLimitFields?.map((betLimitField, index) => (
                <BetLimitRow
                    key={betLimitField._id}
                    currencyField={currencyField}
                    currencyFieldIndex={currencyFieldIndex}
                    isCurrencyChecked={isCurrencyChecked}
                    currencyFieldName={currencyFieldName}
                    betLimitField={betLimitField}
                    betLimitFieldIndex={index}
                    betLimitFields={betLimitFields}
                    showBetLimitDefault={showBetLimitDefault}
                    disabled={disabled}
                />
            ))}
        </>
    )
}

const BetLimitRow = ({
    currencyFieldIndex,
    currencyFieldName,
    betLimitField,
    betLimitFieldIndex,
    betLimitFields = [],
    showBetLimitDefault = true,
    disabled
}) => {
    let {
        register,
        watch,
        setValue,
        getValues,
        formState: { errors },
        clearErrors,
        trigger
    } = useFormContext()
    let currencyCheckedFieldName = toCurrencyCheckedFieldName(
        currencyFieldName,
        currencyFieldIndex
    )
    let betLimitCheckedFieldName = toBetLimitCheckedFieldName({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndex
    })
    let {
        ref: betLimitCheckedRef,
        onChange: onBetLimitCheckedChange,
        ...betLimitCheckedFormProps
    } = register(betLimitCheckedFieldName, {
        validate: () => {
            if (isCurrencyChecked) {
                let checked = anyCheckedBetLimit({
                    currencyFieldName,
                    currencyFieldIndex,
                    betLimitFields,
                    getValues
                })

                return !checked ? betLimitField.currencyCode : true //error message
            }
        }
    })

    let isCurrencyChecked = watch(currencyCheckedFieldName)
    let isBetLimitChecked = watch(betLimitCheckedFieldName)
    let visibility = isCurrencyChecked ? "visible" : "collapse"

    useEffect(() => {
        if (!isCurrencyChecked) {
            setValue(betLimitCheckedFieldName, false)
            clearErrors(betLimitCheckedFieldName)
        }
    }, [isCurrencyChecked])

    let checkedCheckboxError = get(errors, betLimitCheckedFieldName)
    let checkedCheckboxState = checkedCheckboxError && "warning"

    return useMemo(() => {
        let currencyBetLimitIdFieldName = `${currencyFieldName}.${currencyFieldIndex}.betLimits.${betLimitFieldIndex}.id`
        let currencyBetLimitCurrencyCodeFieldName = `${currencyFieldName}.${currencyFieldIndex}.betLimits.${betLimitFieldIndex}.currencyCode`

        return (
            <Table.Row visibility={visibility}>
                <Table.Cell></Table.Cell>
                <Table.Cell>
                    <Box>
                        <Input
                            type="hidden"
                            name={currencyBetLimitIdFieldName}
                            value={betLimitField.id}
                        />
                        <Input
                            type="hidden"
                            name={currencyBetLimitCurrencyCodeFieldName}
                            value={betLimitField.currencyCode}
                        />
                        {disabled && (
                            <>
                                <Checkbox
                                    display="none"
                                    checked={betLimitField.checked}
                                    checkboxRef={betLimitCheckedRef}
                                    {...betLimitCheckedFormProps}
                                />
                                <Checkbox
                                    disabled
                                    checked={betLimitField.checked}
                                />
                            </>
                        )}
                        {!disabled && (
                            <Checkbox
                                checkboxRef={betLimitCheckedRef}
                                state={checkedCheckboxState}
                                defaultChecked={betLimitField.checked}
                                {...betLimitCheckedFormProps}
                                onChange={(event) => {
                                    onBetLimitCheckedChange(event)

                                    let checked = event.target.checked
                                    setValue(
                                        betLimitCheckedFieldName,
                                        event.target.checked
                                    )

                                    if (checked && checkedCheckboxState) {
                                        clearBetLimitCheckedErrors({
                                            currencyFieldName,
                                            currencyFieldIndex,
                                            betLimitFields,
                                            clearErrors
                                        })
                                    }

                                    if (showBetLimitDefault) {
                                        autoCurrencyBetLimitDefault({
                                            betLimitChecked: checked,
                                            betLimitFieldIndex,
                                            currencyFieldName,
                                            currencyFieldIndex,
                                            betLimitFields,
                                            getValues,
                                            setValue
                                        })

                                        validateHasDefaultBetLimit({
                                            currencyFieldIndex,
                                            currencyFieldName,
                                            betLimitFields,
                                            trigger
                                        })
                                    }
                                }}
                            />
                        )}
                    </Box>
                </Table.Cell>
                <Table.Cell textAlign="center">
                    {betLimitField?.minBet}
                </Table.Cell>
                <Table.Cell textAlign="center">
                    {betLimitField?.maxBet}
                </Table.Cell>
                <Table.Cell textAlign="center">
                    {betLimitField?.maxBetPerMatch}
                </Table.Cell>
                {showBetLimitDefault && (
                    <Table.HeadCell textAlign="center">
                        <Box alignX="center">
                            <CurrencyBetLimitDefaultSwitch
                                currencyFieldIndex={currencyFieldIndex}
                                currencyFieldName={currencyFieldName}
                                betLimitField={betLimitField}
                                betLimitFieldIndex={betLimitFieldIndex}
                                betLimitFields={betLimitFields}
                                disabled={disabled}
                            />
                        </Box>
                    </Table.HeadCell>
                )}
            </Table.Row>
        )
    }, [isCurrencyChecked, isBetLimitChecked, checkedCheckboxState])
}

const autoCurrencyBetLimitDefault = ({
    betLimitChecked = false,
    betLimitFieldIndex,
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    getValues,
    setValue
}) => {
    let otherCheckedBetLimitFieldIndexPairs =
        filterOtherCheckedBetLimitFieldIndexPairs(betLimitFieldIndex, {
            currencyFieldName,
            currencyFieldIndex,
            betLimitFields,
            getValues
        })

    let hasDefaultBetLimit = anyDefaultBetLimit({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndexPairs: otherCheckedBetLimitFieldIndexPairs,
        getValues
    })

    if (betLimitChecked && !hasDefaultBetLimit) {
        setBetLimitDefault({
            betLimitFieldIndex,
            currencyFieldName,
            currencyFieldIndex,
            setValue
        })
    }

    if (!betLimitChecked) {
        clearBetLimitDefault({
            betLimitFieldIndex,
            currencyFieldName,
            currencyFieldIndex,
            setValue
        })

        //auto set bet limit default when there's only 1 option
        if (otherCheckedBetLimitFieldIndexPairs.length === 1) {
            let [field, index] = otherCheckedBetLimitFieldIndexPairs[0]
            setBetLimitDefault({
                betLimitFieldIndex: index,
                currencyFieldName,
                currencyFieldIndex,
                setValue
            })
        }
    }
}

const setBetLimitIsDefaultField = (
    isDefault,
    { betLimitFieldIndex, currencyFieldName, currencyFieldIndex, setValue }
) => {
    let isDefaultFieldName = toBetLimitIsDefaultFieldName({
        betLimitFieldIndex,
        currencyFieldName,
        currencyFieldIndex
    })
    setValue(isDefaultFieldName, isDefault)
}

const setBetLimitDefault = ({
    betLimitFieldIndex,
    currencyFieldName,
    currencyFieldIndex,
    setValue
}) =>
    setBetLimitIsDefaultField(true, {
        betLimitFieldIndex,
        currencyFieldName,
        currencyFieldIndex,
        setValue
    })

const clearBetLimitDefault = ({
    betLimitFieldIndex,
    currencyFieldName,
    currencyFieldIndex,
    setValue
}) =>
    setBetLimitIsDefaultField(false, {
        betLimitFieldIndex,
        currencyFieldName,
        currencyFieldIndex,
        setValue
    })

const mapFilterCheckedBetLimitFieldIndexPairs = (
    predicate,
    { currencyFieldName, currencyFieldIndex, betLimitFields, getValues }
) => {
    return betLimitFields
        .map((betLimitField, index) => [betLimitField, index])
        .filter(([betLimitField, index]) => {
            let name = toBetLimitCheckedFieldName({
                currencyFieldName,
                currencyFieldIndex,
                betLimitFieldIndex: index
            })
            let checked = getValues(name)
            if (!checked) {
                return false
            }
            return predicate(betLimitField, index)
        })
}

const filterOtherCheckedBetLimitFieldIndexPairs = (
    dropBetLimitIndex,
    { currencyFieldName, currencyFieldIndex, betLimitFields, getValues }
) => {
    return mapFilterCheckedBetLimitFieldIndexPairs(
        (betLimitField, index) => index !== dropBetLimitIndex,
        { currencyFieldName, currencyFieldIndex, betLimitFields, getValues }
    )
}

const clearIsDefaultBetLimits = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    setValue,
    clearErrors
}) => {
    betLimitFields.forEach((betLimitField, betLimitFieldIndex) => {
        clearBetLimitDefault({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex,
            setValue
        })

        clearBetLimitIsDefaultError({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex,
            clearErrors
        })
    })
}

const clearBetLimitIsDefaultError = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndex,
    clearErrors
}) => {
    let name = toBetLimitIsDefaultFieldName({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndex
    })
    clearErrors(name)
}

const clearBetLimitCheckedErrors = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    clearErrors
}) => {
    let names = betLimitFields.map((betLimitField, betLimitFieldIndex) =>
        toBetLimitCheckedFieldName({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex
        })
    )

    clearErrors(names)
}

const toBetLimitCheckedFieldName = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndex
}) => {
    return `${currencyFieldName}.${currencyFieldIndex}.betLimits.${betLimitFieldIndex}.checked`
}

const toBetLimitIsDefaultFieldName = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndex
}) => {
    return `${currencyFieldName}.${currencyFieldIndex}.betLimits.${betLimitFieldIndex}.isDefault`
}

const isBetLimitChecked = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndex,
    getValues
}) => {
    let betLimitCheckedFieldName = toBetLimitCheckedFieldName({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndex
    })
    return getValues(betLimitCheckedFieldName)
}

const isDefaultBetLimit = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndex,
    getValues
}) => {
    let isDefaultName = toBetLimitIsDefaultFieldName({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndex
    })
    return getValues(isDefaultName)
}

const anyDefaultBetLimit = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFieldIndexPairs = [],
    getValues
}) => {
    return !!betLimitFieldIndexPairs.find(
        ([betLimitField, betLimitFieldIndex]) => {
            let isDefault = isDefaultBetLimit({
                currencyFieldName,
                currencyFieldIndex,
                betLimitFieldIndex,
                getValues
            })

            return isDefault
        }
    )
}

const anyCheckedBetLimit = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    getValues
}) => {
    return !!betLimitFields.find((betLimitField, betLimitFieldIndex) =>
        isBetLimitChecked({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex,
            getValues
        })
    )
}

const CurrencyBetLimitDefaultSwitch = ({
    currencyFieldIndex,
    currencyFieldName,
    betLimitField,
    betLimitFieldIndex,
    betLimitFields = [],
    disabled
}) => {
    let { register, setValue, getValues, watch, errors, trigger, clearErrors } =
        useFormContext()

    let currencyCheckedFieldName = toCurrencyCheckedFieldName(
        currencyFieldName,
        currencyFieldIndex
    )
    let isCurrencyChecked = watch(currencyCheckedFieldName)

    let isDefaultFieldName = toBetLimitIsDefaultFieldName({
        currencyFieldName,
        currencyFieldIndex,
        betLimitFieldIndex
    })

    let {
        ref: isDefaultRef,
        onChange: onIsDefaultChange,
        ...isDefaultFormProps
    } = register?.(isDefaultFieldName, {
        validate: () => {
            if (isCurrencyChecked) {
                let defaultBetLimitCount = countDefaultBetLimit({
                    currencyFieldName,
                    currencyFieldIndex,
                    betLimitFields,
                    getValues
                })

                if (defaultBetLimitCount === 0 || defaultBetLimitCount > 1) {
                    return betLimitField.currencyCode
                }

                return true //must has one default bet limit
            }
        }
    })

    useEffect(() => {
        if (!isCurrencyChecked) {
            setValue(isDefaultFieldName, false)
            clearErrors(isDefaultFieldName)
        }
    }, [isCurrencyChecked])

    let noDefaultError = get(errors, isDefaultFieldName)
    let state = noDefaultError && "warning"

    let editable = !disabled
    return (
        <>
            {disabled && (
                <>
                    <Switch
                        display="none"
                        switchRef={isDefaultRef}
                        {...isDefaultFormProps}
                    />
                    <Switch disabled checked={betLimitField.isDefault} />
                </>
            )}
            {editable && (
                <Switch
                    switchRef={isDefaultRef}
                    defaultChecked={betLimitField.isDefault}
                    state={state}
                    {...isDefaultFormProps}
                    onChange={(event) => {
                        onIsDefaultChange(event)
                        let checked = event.target.checked
                        setValue(isDefaultFieldName, checked)

                        if (checked) {
                            setBetLimitChecked({
                                betLimitFieldIndex,
                                currencyFieldName,
                                currencyFieldIndex,
                                setValue,
                                checked: true
                            })
                            validateHasDefaultBetLimit({
                                currencyFieldName,
                                currencyFieldIndex,
                                betLimitFields,
                                trigger
                            })
                        }

                        autoToggleIsDefault({
                            isDefaultChecked: checked,
                            betLimitFieldIndex,
                            currencyFieldName,
                            currencyFieldIndex,
                            betLimitFields,
                            getValues,
                            setValue
                        })
                    }}
                />
            )}
        </>
    )
}

const countDefaultBetLimit = ({
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    getValues
}) => {
    let defaultBetLimits = betLimitFields.filter(
        (field, betLimitFieldIndex) => {
            let name = toBetLimitIsDefaultFieldName({
                currencyFieldName,
                currencyFieldIndex,
                betLimitFieldIndex
            })
            return getValues(name)
        }
    )
    return defaultBetLimits.length
}

const validateHasDefaultBetLimit = ({
    trigger,
    currencyFieldIndex,
    currencyFieldName,
    betLimitFields = []
}) => {
    let names = betLimitFields.map((field, betLimitFieldIndex) =>
        toBetLimitIsDefaultFieldName({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex
        })
    )

    trigger(names)
}

const setBetLimitChecked = ({
    checked = false,
    betLimitFieldIndex,
    currencyFieldName,
    currencyFieldIndex,
    setValue
}) => {
    let field = toBetLimitCheckedFieldName({
        betLimitFieldIndex,
        currencyFieldName,
        currencyFieldIndex
    })
    setValue(field, checked)
}

const autoToggleIsDefault = ({
    isDefaultChecked = false,
    betLimitFieldIndex,
    currencyFieldName,
    currencyFieldIndex,
    betLimitFields = [],
    getValues,
    setValue
}) => {
    let restCheckedBetLimitFieldIndexPairs =
        filterOtherCheckedBetLimitFieldIndexPairs(betLimitFieldIndex, {
            currencyFieldName,
            currencyFieldIndex,
            betLimitFields,
            getValues
        })

    let hasOtherDefaultBetLimit = anyDefaultBetLimitFromPairs(
        restCheckedBetLimitFieldIndexPairs,
        {
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndexPairs: restCheckedBetLimitFieldIndexPairs,
            getValues
        }
    )

    if (isDefaultChecked && hasOtherDefaultBetLimit) {
        clearAllBetLimitDefaults({
            betLimitFieldIndexPairs: restCheckedBetLimitFieldIndexPairs,
            currencyFieldName,
            currencyFieldIndex,
            setValue
        })
    }

    //auto set bet limit default when there's only 1 option
    if (!isDefaultChecked && restCheckedBetLimitFieldIndexPairs.length === 1) {
        let [field, index] = restCheckedBetLimitFieldIndexPairs[0]
        setBetLimitDefault({
            betLimitFieldIndex: index,
            currencyFieldName,
            currencyFieldIndex,
            setValue
        })
    }
}

const anyDefaultBetLimitFromPairs = (
    betLimitFieldIndexPairs = [],
    { currencyFieldName, currencyFieldIndex, getValues }
) => {
    return !!betLimitFieldIndexPairs.find(([field, betLimitFieldIndex]) =>
        isDefaultBetLimit({
            currencyFieldName,
            currencyFieldIndex,
            betLimitFieldIndex,
            getValues
        })
    )
}

const clearAllBetLimitDefaults = ({
    betLimitFieldIndexPairs,
    currencyFieldName,
    currencyFieldIndex,
    setValue
}) => {
    betLimitFieldIndexPairs.forEach(([betLimitField, index]) => {
        clearBetLimitDefault({
            betLimitFieldIndex: index,
            currencyFieldName,
            currencyFieldIndex,
            setValue
        })
    })
}

export const flatCheckedCurrencyBetLimits = (toBetLimit, currency = []) => {
    return currency.reduce(
        (list, { checked: checkedCurrency, betLimits = [] }) => {
            let checkedBetLimits = betLimits
                .filter(({ checked }) => checked)
                .map(toBetLimit)

            if (checkedCurrency) {
                list.push(...checkedBetLimits)
            }
            return list
        },
        []
    )
}

export const flatCheckedCurrencyBetLimitIds = (currency = []) =>
    flatCheckedCurrencyBetLimits(({ id }) => parseInt(id), currency)

export default CurrencyBetLimitFieldsBox
