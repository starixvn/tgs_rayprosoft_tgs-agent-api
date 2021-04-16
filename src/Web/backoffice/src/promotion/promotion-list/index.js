import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Text, Flex } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId,
    useUserBrandId
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
import { Trans, useTrans } from "../../i18n"
import {
    AddPromotionDisclosure,
    EditPromotionDisclosure,
    ViewPromotionDisclosure
} from "./edit"
import equals from "fast-deep-equal"
import { usePromotionList } from "./client"
import BrandBar from "../../report/BrandBar"
import { formatDateTime, formatDayMonthYear } from "../../date/format"

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
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        name: "",
        brandId
    })

    useEffect(() => {
        setState({ pageIndex: 0, brandId })
    }, [brandId])

    let { isLoading, data, error, mutate, deleteCache } = usePromotionList(
        toApiParams(state)
    )

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="活动管理" en="Promotion" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        code={state?.code}
                        name={state?.name}
                        search={(data) => {
                            let newState = {
                                pageIndex: 0,
                                name: data?.name
                            }
                            if (
                                equals(newState, {
                                    pageIndex: state?.pageIndex,
                                    name: state?.name
                                })
                            ) {
                                mutate()
                            }
                            setState(newState)
                        }}
                        isLoading={isLoading}
                    />
                    <AddPromotionDisclosure onSubmitted={mutate} />
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

const SearchBar = ({ name = "", search, isLoading }) => {
    let namePlaceholder = useTrans({
        zh: "名称",
        en: "Name"
    })
    let formMethods = useForm({
        defaultValues: { name }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField
                        name="name"
                        showEmptyFieldValueAddonAfter
                        placeholder={namePlaceholder}
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    //TODO: view or update menu
    return useMemo(
        () => [
            textColumn({
                accessor: "name",
                Header: <Trans zh="活动名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                id: "promotionTypeGrup",
                accessor: (row) => row,
                Header: <Trans zh="活动类型" en="Promotion Type" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return <Text>TODO: map to group</Text>
                }
            }),
            textColumn({
                accessor: "promotionType",
                Header: <Trans zh="活动子类型" en="Promotion Subtype" />,
                disableSortBy: true
            }),
            textColumn({
                id: "promotionTime",
                accessor: (row) => row,
                Header: <Trans zh="活动时间" en="Promotion Time" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let from = formatDayMonthYear(value?.from)
                    let to = formatDayMonthYear(value?.to)
                    return `${from} - ${to}`
                }
            }),
            textColumn({
                id: "gameTypeGroup",
                accessor: (row) => row,
                Header: <Trans zh="游戏类型" en="Game Type Group" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "gameType",
                Header: <Trans zh="游戏子类型" en="Game Type" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "gameProviderIds",
                Header: <Trans zh="活动厂商" en="Game Providers" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "vipLevels",
                Header: <Trans zh="活动用户群" en="VIP Levels" />,
                disableSortBy: true
            }),
            textColumn({
                id: "amounts",
                accessor: (row) => row,
                Header: <Trans zh="活动费用" en="Amount" />,
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
                            <EditPromotionDisclosure
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
