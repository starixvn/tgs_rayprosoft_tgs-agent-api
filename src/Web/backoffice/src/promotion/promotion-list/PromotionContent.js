import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Text,
    Button,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import { Update as UpdateMenu, View as ViewMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import {
    Controller,
    useForm,
    useFormContext,
    useWatch,
    useFieldArray
} from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddPromotion, useUpdatePromotion } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import AlertDialog from "../../dialog/Alert"
import Checkbox from "../../checkbox"
import {
    GameTypeGroupSelect,
    GameTypeSelect,
    useGameTypeGroupValueOf
} from "../../game/GameType"
import Separator from "../../layout/Separator"
import {
    PromotionTypeGroup,
    PromotionTypeGroupSelect,
    PromotionTypeSelect,
    usePromotionGroupValueOf
} from "../PromotionType"
import { toCheckedGameProviderIds } from "../../game/providers/GameProviderCheckboxGroup"
import { BrandGameProviderCheckboxGroupTable } from "../../game/brandproviders/BrandGameProviderCheckboxGroup"
import parseISO from "date-fns/parseISO"
import { today } from "../../date/range"
import { useBrandGameProviderList } from "../../game/brandproviders/client"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"
import {
    NumberInput,
    NumberInputField,
    PercentageNumberInputField
} from "../../input/NumberInput"
import { Switch } from "../../switch/Switch"
import { HtmlEditor } from "../../text/editor/HtmlEditor"
import { Tabs, Tab, TabPanel, TabList, useTabState } from "../../tab"
import Backdrop from "../../dialog/Backdrop"
import { ConfirmText } from "../../button/ActionButtons"
import head from "../../list/head"

export const PromotionContentDisclosure = ({
    name = "contents",
    readOnly = false,
    ...props
}) => {
    let { control, setValue } = useFormContext()
    let value = useWatch({
        control,
        name
    })

    let dialog = useModalState()
    let editable = !readOnly

    return (
        <Controller
            name={name}
            control={control}
            defaultValue={value}
            shouldUnregister={true}
            render={({
                field: { value, name, ...fieldProps },
                fieldState,
                formState
            }) => (
                <>
                    {readOnly && <ViewDisclosure {...dialog} />}
                    {editable && <EditDisclosure {...dialog} />}
                    {dialog.visible && (
                        <PromotionContentTabsDialog
                            value={value}
                            onChange={(data) => {
                                setValue(name, data)
                            }}
                            dialog={dialog}
                        />
                    )}
                </>
            )}
        />
    )
}

const ContentDialog = ({ value = {}, readOnly, onChange, dialog }) => {
    let editTitle = useTrans({
        zh: "编辑内容",
        en: "Edit Content"
    })
    let viewTitle = useTrans({
        zh: "查看内容",
        en: "View Content"
    })

    let defaultValues = clone(value)
    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={defaultValues}
                onSubmit={(data) => {
                    console.log("onSubmit", data)
                    if (!readOnly) {
                        onChange?.(data)
                    }
                    dialog.hide()
                }}
                stopPropagation={true}
                dialog={dialog}
                hideBackdrop={true}
                title={readOnly ? viewTitle : editTitle}
                submitText={<ConfirmText />}
                width="80vw"
                maxWidth="80vw"
            >
                <TabsBody readOnly={readOnly} />
            </FormDialog>
        </Backdrop>
    )
}

const FormBody = ({ readOnly }) => {
    return (
        <FieldValues>
            <NormalLabel>
                <Trans zh="活动标题" en="Title" />
            </NormalLabel>
            <Box>
                <Input name={`.title`} readOnly={readOnly} />
            </Box>

            <NormalLabel>
                <Trans zh="活动子标题" en="Subtitle" />
            </NormalLabel>
            <Box>
                <Input name={`subtitle`} readOnly={readOnly} />
            </Box>

            <NormalLabel>
                <Trans zh="活动Banner" en="Banner" />
            </NormalLabel>
            <Box>//TODO: upload img</Box>

            <NormalLabel>
                <Trans zh="活动内容" en="Content" />
            </NormalLabel>
            <Box>
                <HtmlEditor
                    active={isActive}
                    name={`content`}
                    defaultValue={record?.content}
                    readOnly={readOnly}
                />
            </Box>

            <NormalLabel>
                <Trans zh="活动条款与条规" en="Terms" />
            </NormalLabel>
            <Box>
                <HtmlEditor
                    active={isActive}
                    name={`terms`}
                    defaultValue={record?.terms}
                    readOnly={readOnly}
                />
            </Box>
        </FieldValues>
    )
}

const TabsFieldName = "record"
const PromotionContentTabsDialog = ({
    value = [],
    readOnly,
    onChange,
    dialog
}) => {
    let editTitle = useTrans({
        zh: "编辑内容",
        en: "Edit Content"
    })
    let viewTitle = useTrans({
        zh: "查看内容",
        en: "View Content"
    })

    let defaultValues = {}
    Reflect.set(defaultValues, TabsFieldName, clone(value))
    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={defaultValues}
                onSubmit={(data) => {
                    console.log("onSubmit", data)
                    if (!readOnly) {
                        onChange?.(data)
                    }
                    dialog.hide()
                }}
                stopPropagation={true}
                dialog={dialog}
                hideBackdrop={true}
                title={readOnly ? viewTitle : editTitle}
                submitText={<ConfirmText />}
                width="80vw"
                maxWidth="80vw"
            >
                <TabsBody readOnly={readOnly} />
            </FormDialog>
        </Backdrop>
    )
}

const TabsBody = ({ readOnly }) => {
    let { control, watch } = useFormContext()
    let { fields } = useFieldArray({
        name: TabsFieldName,
        control,
        keyName: "_id"
    })
    let watchFieldArray = watch(TabsFieldName)
    let controlledFields = fields.map((field, index) => {
        return {
            ...field,
            ...watchFieldArray[index]
        }
    })

    let selectedId = head(fields)?.cultureCode
    let tab = useTabState({
        selectedId
    })

    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                {fields.map(({ cultureCode }) => (
                    <Tab tabId={cultureCode} {...tab}>
                        {cultureCode}
                    </Tab>
                ))}
            </TabList>
            {controlledFields.map((content, index) => (
                <ContentPanel
                    name={TabsFieldName}
                    record={content}
                    index={index}
                    readOnly={readOnly}
                    {...tab}
                />
            ))}
        </Tabs>
    )
}

const ContentPanel = ({ name, record = {}, index, readOnly, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === record.cultureCode

    return (
        <TabPanel tabId={record.cultureCode} {...props}>
            <TabContentBody
                name={TabsFieldName}
                index={index}
                record={record}
                readOnly={readOnly}
                isActive={isActive}
            />
        </TabPanel>
    )
}

const TabContentBody = ({ name, index, record = {}, isActive, readOnly }) => {
    return (
        <FieldValues>
            <NormalLabel>
                <Trans zh="活动标题" en="Title" />
            </NormalLabel>
            <Box>
                <Input
                    name={`${name}.${index}.title`}
                    defaultValue={record?.title}
                    readOnly={readOnly}
                />
            </Box>

            <NormalLabel>
                <Trans zh="活动子标题" en="Subtitle" />
            </NormalLabel>
            <Box>
                <Input
                    name={`${name}.${index}.subtitle`}
                    defaultValue={record?.subtitle}
                    readOnly={readOnly}
                />
            </Box>

            <NormalLabel>
                <Trans zh="活动Banner" en="Banner" />
            </NormalLabel>
            <Box>//TODO: upload img</Box>

            <NormalLabel>
                <Trans zh="活动内容" en="Content" />
            </NormalLabel>
            <Box>
                <HtmlEditor
                    active={isActive}
                    name={`${name}.${index}.content`}
                    defaultValue={record?.content}
                    readOnly={readOnly}
                />
            </Box>

            <NormalLabel>
                <Trans zh="活动条款与条规" en="Terms" />
            </NormalLabel>
            <Box>
                <HtmlEditor
                    active={isActive}
                    name={`${name}.${index}.terms`}
                    defaultValue={record?.terms}
                    readOnly={readOnly}
                />
            </Box>
        </FieldValues>
    )
}
