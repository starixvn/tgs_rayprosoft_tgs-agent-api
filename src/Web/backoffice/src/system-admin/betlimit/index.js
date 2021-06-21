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
import { Code, Update } from "./menu"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { PageContent } from "../../page/Content"
import AlertDialog from "../../dialog/Alert"
import { Trans } from "../../i18n"
import { AddBetLimitDisclosure, EditBetLimitDisclosure } from "./edit"
import equals from "fast-deep-equal"
import { useBetLimitList } from "../../betlimit/client"

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
        keyword: ""
    })

    let { isLoading, data, error, mutate, deleteCache } = useBetLimitList({
        ...toApiParams(state),
        sortby: "id"
    })

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="限红" en="Bet Limit" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        code={state?.code}
                        name={state?.name}
                        search={(data = {}) => {
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                            mutate()
                        }}
                        isLoading={isLoading}
                    />
                    <AddBetLimitDisclosure onSubmitted={mutate} />
                </Flex>
            }
        >
            <Table
                columns={columns}
                data={data?.items}
                initialState={state}
                controlledState={state}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && <AlertDialog {...error} onClickClose={deleteCache} />}
        </PageContent>
    )
}

const SearchBar = ({ keyword = "", search, isLoading }) => {
    let formMethods = useForm({
        defaultValues: { keyword }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField
                        name="keyword"
                        showEmptyFieldValueAddonAfter
                        placeholder="Keyword"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="Id" en="Id" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "currencyCode",
                Header: <Trans zh="Currency Code" en="Currency Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "minBet",
                Header: <Trans zh="Min Bet" en="Min Bet" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "maxBet",
                Header: <Trans zh="Max Bet" en="Max Bet" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "maxBetPerMatch",
                Header: <Trans zh="Max Bet Per Match" en="Max Bet Per Match" />,
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
            })
        ]

        if (hasUpdateAuth) {
            columns.push(
                textColumn({
                    id: "actions",
                    Header: <Trans zh="操作" en="Actions" />,
                    accessor: (row) => row,
                    disableSortBy: true,
                    Cell: ({ value: record }) => {
                        return (
                            <Set orientation="horizontal">
                                <EditBetLimitDisclosure
                                    record={record}
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
