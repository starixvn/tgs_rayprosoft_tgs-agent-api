import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    useModalState,
    Dialog,
    Card,
    Level
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure, {
    AddDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller,
    FormProvider
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { pageCount, Table, toApiParams, useTable } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import dateTimeColumn from "../../table/column/dateTime"
import { View, Update } from "./menu"
import { CopyTextBox } from "../../text/CopyTextBox"
import {
    AddGameProviderConfigDisclosure,
    EditGameProviderConfigDisclosure,
    ViewGameProviderConfigDisclosure
} from "./GameProviderConfig"
import { useGameProviderConfigList } from "../../game/providers/configs/client"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"
import { Switch } from "../../switch/Switch"

export const GameProviderConfigListDisclosure = ({ gameProviderId }) => {
    let dialog = useModalState()
    let title = useTrans({
        zh: "配置列表",
        en: "Config List"
    })

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>{title}</Disclosure>
            {dialog.visible && (
                <ConfigListDialog
                    gameProviderId={gameProviderId}
                    dialog={dialog}
                    title={title}
                />
            )}
        </>
    )
}

const ConfigListDialog = ({ gameProviderId, dialog, title, ...props }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        gameProviderId
    })
    let { pageIndex, pageSize } = state
    let { isLoading, data, error, mutate, deleteCache } =
        useGameProviderConfigList({ ...toApiParams(state), sortby: "name" })

    let items = data?.items
    let columns = useColumns({ items, mutate })

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    return (
        <Dialog.Modal
            title={title}
            showCloseButton
            footer={<CloseButtonFooter alignX="right" dialog={dialog} />}
            {...dialog}
            showCloseButton
            hideOnClickOutside={false}
            placement="top"
            width="80vw"
            maxWidth="80vw"
        >
            <FieldStack>
                <ActionBar
                    gameProviderId={gameProviderId}
                    isLoading={isLoading}
                    search={(data = {}) => {
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                        mutate()
                    }}
                    mutate={mutate}
                />
                <Table
                    columns={columns}
                    data={items}
                    initialState={state}
                    controlledState={state}
                    pageCount={pageCount(state.pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                />
            </FieldStack>
        </Dialog.Modal>
    )
}

const ActionBar = ({ gameProviderId, search, mutate, ...props }) => {
    let formMethods = useForm({
        defaultValues: {}
    })

    return (
        <Level>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(search)}>
                    {/* <Set>
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                        />
                    </Set> */}
                </form>
            </FormProvider>
            <Set orientation="horizontal">
                <AddGameProviderConfigDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={mutate}
                />
            </Set>
        </Level>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="ID" en="ID" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="Name" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "description",
                Header: <Trans zh="Description" en="Description" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "clientId",
                Header: <Trans zh="Client Id" en="Client Id" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => <CopyTextBox value={value} />
            }),
            textColumn({
                accessor: "secretKey",
                Header: <Trans zh="Secret Key" en="secret Key" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => <CopyTextBox value={value} />
            }),
            textColumn({
                accessor: "isDefault",
                Header: <Trans zh="Default" en="Default" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => <Switch checked={value} />
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="最新更新" en="Updated On" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "updatedByUsername",
                Header: <Trans zh="更改者" en="Updated By" />,
                disableSortBy: true
            })
        ]

        if (hasUpdateAuth) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    Header: <Trans zh="操作" en="Actions" />,
                    disableSortBy: true,
                    Cell: ({ value: record }) => {
                        return (
                            <Set orientation="horizontal" alignX="center">
                                <ViewGameProviderConfigDisclosure
                                    id={record.id}
                                />
                                <EditGameProviderConfigDisclosure
                                    id={record.id}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}
