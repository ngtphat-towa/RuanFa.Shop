using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RuanFa.Shop.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Intial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "attribute_groups",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attribute_groups", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "attributes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                is_required = table.Column<bool>(type: "boolean", nullable: false),
                display_on_frontend = table.Column<bool>(type: "boolean", nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                is_filterable = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attributes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "categories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                url_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                include_in_nav = table.Column<bool>(type: "boolean", nullable: false),
                position = table.Column<short>(type: "smallint", nullable: true),
                show_products = table.Column<bool>(type: "boolean", nullable: false),
                image_alt = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                description = table.Column<string>(type: "text", nullable: true),
                meta_title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                meta_keywords = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                meta_description = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_categories", x => x.id);
                table.ForeignKey(
                    name: "fk_categories_categories_parent_id",
                    column: x => x.parent_id,
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true),
                name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "todo_lists",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                colour = table.Column<string>(type: "TEXT", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_lists", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true),
                profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: true),
                security_stamp = table.Column<string>(type: "text", nullable: true),
                concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                phone_number = table.Column<string>(type: "text", nullable: true),
                phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                access_failed_count = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "products",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                weight = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                tax_class = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                group_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
                table.ForeignKey(
                    name: "fk_products_attribute_groups_group_id",
                    column: x => x.group_id,
                    principalTable: "attribute_groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "attribute_group_attributes",
            columns: table => new
            {
                attribute_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attribute_group_attributes", x => new { x.attribute_group_id, x.attribute_id });
                table.ForeignKey(
                    name: "fk_attribute_group_attributes_attribute_groups_attribute_group",
                    column: x => x.attribute_group_id,
                    principalTable: "attribute_groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_attribute_group_attributes_attributes_attribute_id",
                    column: x => x.attribute_id,
                    principalTable: "attributes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "attribute_options",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                attribute_code = table.Column<string>(type: "text", nullable: false),
                option_text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attribute_options", x => x.id);
                table.ForeignKey(
                    name: "fk_attribute_options_attributes_attribute_id",
                    column: x => x.attribute_id,
                    principalTable: "attributes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "role_claims",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                role_id = table.Column<Guid>(type: "uuid", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_role_claims_roles_role_id",
                    column: x => x.role_id,
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "todo_items",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                priority = table.Column<int>(type: "integer", nullable: false),
                done = table.Column<bool>(type: "boolean", nullable: false),
                done_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                list_id = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_todo_items", x => x.id);
                table.ForeignKey(
                    name: "fk_todo_items_todo_lists_list_id",
                    column: x => x.list_id,
                    principalTable: "todo_lists",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_claims",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                claim_type = table.Column<string>(type: "text", nullable: true),
                claim_value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_user_claims_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_logins",
            columns: table => new
            {
                login_provider = table.Column<string>(type: "text", nullable: false),
                provider_key = table.Column<string>(type: "text", nullable: false),
                provider_display_name = table.Column<string>(type: "text", nullable: true),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                table.ForeignKey(
                    name: "fk_user_logins_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_profiles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                username = table.Column<string>(type: "text", nullable: true),
                email = table.Column<string>(type: "text", nullable: false),
                full_name = table.Column<string>(type: "text", nullable: false),
                phone_number = table.Column<string>(type: "text", nullable: true),
                gender = table.Column<int>(type: "integer", nullable: false),
                date_of_birth = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                addresses = table.Column<string>(type: "TEXT", nullable: false),
                preferences = table.Column<string>(type: "TEXT", nullable: false),
                wishlist = table.Column<string>(type: "TEXT", nullable: false),
                loyalty_points = table.Column<int>(type: "integer", nullable: false),
                marketing_consent = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_profiles", x => x.id);
                table.ForeignKey(
                    name: "fk_user_profiles_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_roles",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                role_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                table.ForeignKey(
                    name: "fk_user_roles_roles_role_id",
                    column: x => x.role_id,
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_tokens",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                login_provider = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                table.ForeignKey(
                    name: "fk_user_tokens_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_categories",
            columns: table => new
            {
                category_id = table.Column<Guid>(type: "uuid", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_categories", x => new { x.product_id, x.category_id });
                table.ForeignKey(
                    name: "fk_product_categories_categories_category_id",
                    column: x => x.category_id,
                    principalTable: "categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_categories_product_product_id",
                    column: x => x.product_id,
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "variants",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                price_offset = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                stock_quantity = table.Column<int>(type: "integer", nullable: false),
                low_stock_threshold = table.Column<int>(type: "integer", nullable: false),
                stock_status = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_variants", x => x.id);
                table.ForeignKey(
                    name: "fk_variants_products_product_id",
                    column: x => x.product_id,
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "order",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_order", x => x.id);
                table.ForeignKey(
                    name: "fk_order_user_profiles_id",
                    column: x => x.id,
                    principalTable: "user_profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "product_images",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                image_image_type = table.Column<int>(type: "integer", nullable: false),
                image_alt = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: false),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                image_mime_type = table.Column<string>(type: "text", nullable: true),
                image_width = table.Column<int>(type: "integer", nullable: true),
                image_height = table.Column<int>(type: "integer", nullable: true),
                image_file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                product_id = table.Column<Guid>(type: "uuid", nullable: false),
                variant_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_images", x => x.id);
                table.ForeignKey(
                    name: "fk_product_images_product_variant_variant_id",
                    column: x => x.variant_id,
                    principalTable: "variants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_product_images_products_product_id",
                    column: x => x.product_id,
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "stock_movement",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<int>(type: "integer", nullable: false),
                movement_type = table.Column<int>(type: "integer", nullable: false),
                reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                notes = table.Column<string>(type: "text", nullable: true),
                variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stock_movement", x => x.id);
                table.ForeignKey(
                    name: "fk_stock_movement_product_variant_variant_id",
                    column: x => x.variant_id,
                    principalTable: "variants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "variants_attribute_options",
            columns: table => new
            {
                variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                attribute_option_id = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "text", nullable: true),
                updated_by = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_variants_attribute_options", x => new { x.variant_id, x.attribute_option_id });
                table.ForeignKey(
                    name: "fk_variants_attribute_options_attribute_options_attribute_opti",
                    column: x => x.attribute_option_id,
                    principalTable: "attribute_options",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_variants_attribute_options_product_variant_variant_id",
                    column: x => x.variant_id,
                    principalTable: "variants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_attribute_group_attributes_attribute_id",
            table: "attribute_group_attributes",
            column: "attribute_id");

        migrationBuilder.CreateIndex(
            name: "ix_attribute_groups_name",
            table: "attribute_groups",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_attribute_options_attribute_code_option_text",
            table: "attribute_options",
            columns: new[] { "attribute_code", "option_text" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_attribute_options_attribute_id",
            table: "attribute_options",
            column: "attribute_id");

        migrationBuilder.CreateIndex(
            name: "ix_attributes_code",
            table: "attributes",
            column: "code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_attributes_name",
            table: "attributes",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_categories_name",
            table: "categories",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_categories_parent_id",
            table: "categories",
            column: "parent_id");

        migrationBuilder.CreateIndex(
            name: "ix_categories_url_key",
            table: "categories",
            column: "url_key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_product_categories_category_id",
            table: "product_categories",
            column: "category_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_categories_product_id",
            table: "product_categories",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_product_images_product_id_variant_id",
            table: "product_images",
            columns: new[] { "product_id", "variant_id" });

        migrationBuilder.CreateIndex(
            name: "ix_product_images_variant_id",
            table: "product_images",
            column: "variant_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_group_id",
            table: "products",
            column: "group_id");

        migrationBuilder.CreateIndex(
            name: "ix_products_name",
            table: "products",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ix_products_sku",
            table: "products",
            column: "sku",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_role_claims_role_id",
            table: "role_claims",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "roles",
            column: "normalized_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_stock_movement_variant_id",
            table: "stock_movement",
            column: "variant_id");

        migrationBuilder.CreateIndex(
            name: "ix_todo_items_list_id",
            table: "todo_items",
            column: "list_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_claims_user_id",
            table: "user_claims",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_logins_user_id",
            table: "user_logins",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_profiles_user_id",
            table: "user_profiles",
            column: "user_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_role_id",
            table: "user_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "users",
            column: "normalized_email");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "users",
            column: "normalized_user_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_variants_product_id",
            table: "variants",
            column: "product_id");

        migrationBuilder.CreateIndex(
            name: "ix_variants_sku",
            table: "variants",
            column: "sku",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_variants_attribute_options_attribute_option_id",
            table: "variants_attribute_options",
            column: "attribute_option_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "attribute_group_attributes");

        migrationBuilder.DropTable(
            name: "order");

        migrationBuilder.DropTable(
            name: "product_categories");

        migrationBuilder.DropTable(
            name: "product_images");

        migrationBuilder.DropTable(
            name: "role_claims");

        migrationBuilder.DropTable(
            name: "stock_movement");

        migrationBuilder.DropTable(
            name: "todo_items");

        migrationBuilder.DropTable(
            name: "user_claims");

        migrationBuilder.DropTable(
            name: "user_logins");

        migrationBuilder.DropTable(
            name: "user_roles");

        migrationBuilder.DropTable(
            name: "user_tokens");

        migrationBuilder.DropTable(
            name: "variants_attribute_options");

        migrationBuilder.DropTable(
            name: "user_profiles");

        migrationBuilder.DropTable(
            name: "categories");

        migrationBuilder.DropTable(
            name: "todo_lists");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "attribute_options");

        migrationBuilder.DropTable(
            name: "variants");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "attributes");

        migrationBuilder.DropTable(
            name: "products");

        migrationBuilder.DropTable(
            name: "attribute_groups");
    }
}
