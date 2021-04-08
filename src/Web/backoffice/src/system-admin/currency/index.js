import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon, Flex } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Code } from "./menu"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { PageContent } from "../../page/Content"
import AlertDialog from "../../dialog/Alert"
import { Trans } from "../../i18n"
import { AddCurrencyDisclosure, EditCurrencyDisclosure } from "./edit"
import { useCurrencyList } from "../../currency/client"
import equals from "fast-deep-equal"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        code: "",
        name: ""
    })

    let { isLoading, data, error, mutate, deleteCache } = useCurrencyList(
        toApiParams(state)
    )

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="币种" en="Currency" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        code={state?.code}
                        name={state?.name}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                code: data?.code,
                                name: data?.name
                            }
                            if (
                                equals(newState, {
                                    pageIndex: state?.pageIndex,
                                    code: state?.code,
                                    name: state?.name
                                })
                            ) {
                                mutate()
                            }
                            setState(newState)
                        }}
                        isLoading={isLoading}
                    />
                    <AddCurrencyDisclosure onSubmitted={mutate} />
                </Flex>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({ code = "", name = "", search, isLoading }) => {
    let formMethods = useForm({
        defaultValues: { code, name }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField
                        name="code"
                        showEmptyFieldValueAddonAfter
                        placeholder="Code"
                    />
                    <InputField
                        name="name"
                        showEmptyFieldValueAddonAfter
                        placeholder="Name"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "code",
                Header: <Trans zh="Code" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="Name" en="name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "nativeName",
                Header: <Trans zh="Native Name" en="Native Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "symbol",
                Header: <Trans zh="Symbol" en="Symbol" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="创建时间" en="Updated Time" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="更改时间" en="Updated Time" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="创建人" en="Created By" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "updatedByUsername",
                Header: <Trans zh="更新人" en="Updated By" />,
                disableSortBy: true
            }),
            textColumn({
                id: "actions",
                Header: <Trans zh="操作" en="Actions" />,
                accessor: (row) => row,
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    return (
                        <Set orientation="horizontal">
                            <EditCurrencyDisclosure
                                record={record}
                                onSubmitted={mutate}
                            />
                        </Set>
                    )
                }
            })
        ],
        [JSON.stringify(items)]
    )
}
